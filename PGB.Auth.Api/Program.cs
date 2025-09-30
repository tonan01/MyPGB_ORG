using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PGB.Auth.Application.Repositories;
using PGB.Auth.Application.Services;
using PGB.Auth.Domain.ValueObjects;
using PGB.Auth.Infrastructure.Data;
using PGB.Auth.Infrastructure.Repositories;
using PGB.Auth.Infrastructure.Services;
using PGB.BuildingBlocks.Application.Extensions;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Application services (MediatR, validators, behaviors, AutoMapper)
builder.Services.AddApplicationServices(Assembly.Load("PGB.Auth.Application"));

// Register JwtTokenService from Infrastructure
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Register Auth DbContext (cập nhật connection string name cho phù hợp)
var conn = builder.Configuration.GetConnectionString("AuthDatabase") ?? "...";
builder.Services.AddDbContextPool<AuthDbContext>(options =>
    options.UseSqlServer(conn, sql =>
    {
        sql.MigrationsAssembly("PGB.Auth.Infrastructure");
        sql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
        sql.CommandTimeout(60);
    })
    .EnableSensitiveDataLogging(builder.Environment.IsDevelopment()));

// Register repositories and domain services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserDomainService, UserDomainService>();

// Register IPasswordHasher implementation
// Nếu bạn đã có class concrete (ví dụ BcryptPasswordHasher) thay vào bên dưới.
// Nếu chưa có, bạn cần implement IPasswordHasher trong PGB.Auth.Infrastructure.
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

// Register SecuritySettings (tạm dùng default, hoặc bind từ configuration nếu bạn có cấu hình)
builder.Services.AddSingleton(SecuritySettings.Default());

// Configure JWT
var jwtSection = builder.Configuration.GetSection("JwtSettings");
var secret = jwtSection["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
var key = Encoding.ASCII.GetBytes(secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSection["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // <- BẮT BUỘC: phải gọi trước UseAuthorization1
app.UseAuthorization();

app.MapControllers();
app.Run();