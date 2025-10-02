using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace PGB.ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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

            // IMPORTANT: Do NOT use UseRouting() with Ocelot
            // Ocelot handles routing internally

            app.UseAuthentication();
            app.UseAuthorization();

            // Use Ocelot middleware - this MUST be last
            app.UseOcelot().Wait();

            app.Run();
        }
    }
}