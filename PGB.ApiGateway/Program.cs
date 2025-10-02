using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.IdentityModel.Tokens.Jwt; // <-- THÊM USING NÀY

namespace PGB.ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // --- THÊM DÒNG NÀY Ở ĐẦU PHƯƠNG THỨC Main ---
            // Tắt tính năng tự động map claim của Microsoft
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            // ---------------------------------------------

            var builder = WebApplication.CreateBuilder(args);

            // ... (phần còn lại của file giữ nguyên)

            // Load ocelot configuration
            builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

            // Add services to the container.
            builder.Services.AddControllers();

            // JWT authentication for gateway
            var jwtSection = builder.Configuration.GetSection("JwtSettings");
            var secret = jwtSection["SecretKey"] ?? throw new InvalidOperationException("JwtSettings:SecretKey not configured");
            var issuer = jwtSection["Issuer"] ?? "PGB_ORG";
            var audience = jwtSection["Audience"] ?? "PGB_ORG_Users";

            var key = System.Text.Encoding.UTF8.GetBytes(secret);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("JwtBearer", options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Ocelot (API Gateway) services
            builder.Services.AddOcelot(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseOcelot().Wait();

            app.Run();
        }
    }
}