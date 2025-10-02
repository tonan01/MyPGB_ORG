
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

            // load ocelot configuration
            builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("ocelot.json", optional: true, reloadOnChange: true);
            });

            // Add services to the container.

            builder.Services.AddControllers();
            // JWT authentication for gateway
            var jwtSection = builder.Configuration.GetSection("JwtSettings");
            var secret = jwtSection["SecretKey"] ?? throw new InvalidOperationException("JwtSettings:SecretKey not configured");
            var issuer = jwtSection["Issuer"] ?? "PGB_ORG";
            var audience = jwtSection["Audience"] ?? "PGB_ORG_Users";

            var key = System.Text.Encoding.UTF8.GetBytes(secret);
            // Use named scheme 'JwtBearer' and set it as the default to match ocelot configuration
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
            .AddJwtBearer("JwtBearer", options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                    ValidateLifetime = true
                };
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

            // If you want only HTTP on local dev, you can remove or comment out UseHttpsRedirection
            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            // Use Ocelot middleware to proxy requests (ensure authentication runs before Ocelot)
            // Start Ocelot after routing and auth middleware
            app.UseOcelot().GetAwaiter().GetResult();

            app.Run();
        }
    }
}
