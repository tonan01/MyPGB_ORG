using Microsoft.EntityFrameworkCore;
using PGB.Auth.Application.Repositories;
using PGB.Auth.Application.Services;
using PGB.Auth.Domain.ValueObjects;
using PGB.Auth.Infrastructure.Data;
using PGB.Auth.Infrastructure.Repositories;
using PGB.Auth.Infrastructure.Services;
using PGB.BuildingBlocks.Application.Extensions;
using System.Reflection;
using PGB.BuildingBlocks.WebApi.Common.Extensions;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services
// Use shared WebApi.Common services
builder.AddWebApiCommon();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register IHttpContextAccessor and CurrentUserService
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<PGB.Auth.Api.Services.CurrentUserService>();
builder.Services.AddScoped<PGB.BuildingBlocks.Domain.Interfaces.ICurrentUserService>(sp => sp.GetRequiredService<PGB.Auth.Api.Services.CurrentUserService>());

// Redis removed — use in-memory behaviors by default

// Register Application services (MediatR, validators, behaviors, AutoMapper)
builder.Services.AddApplicationServices(Assembly.Load("PGB.Auth.Application"));

// Register JwtTokenService from Infrastructure
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Register Auth DbContext (cập nhật connection string name cho phù hợp)
var conn = builder.Configuration.GetConnectionString("AuthDatabase")
          ?? builder.Configuration.GetConnectionString("DefaultConnection")
          ?? throw new InvalidOperationException("DB connection string 'AuthDatabase' or 'DefaultConnection' not configured");
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

// JWT validation removed from Auth API — handled by API Gateway

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use shared middleware (correlation id, etc)
app.UseWebApiCommon();

// Enable EF SQL logging when in development
if (app.Environment.IsDevelopment())
{
    var loggerFactory = app.Services.GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>();
    loggerFactory.CreateLogger("EF").LogInformation("EF logging enabled");
}

app.UseHttpsRedirection();

app.UseAuthentication(); // <- BẮT BUỘC: phải gọi trước UseAuthorization1
app.UseAuthorization();

app.MapControllers();
app.Run();

// Make the implicit Program class available for integration tests
public partial class Program { }