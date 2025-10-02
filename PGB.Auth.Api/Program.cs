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

var builder = WebApplication.CreateBuilder(args);

// Add services
// Use shared WebApi.Common services
builder.AddWebApiCommon();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- PHẦN MỚI: Kích hoạt Authentication và Authorization ---
// Mặc dù không cần handler (vì Gateway đã xác thực),
// các dịch vụ này là cần thiết để middleware và [Authorize] hoạt động.
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
// --- KẾT THÚC PHẦN MỚI ---

// Register IHttpContextAccessor and CurrentUserService
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<PGB.Auth.Api.Services.CurrentUserService>();
builder.Services.AddScoped<PGB.BuildingBlocks.Domain.Interfaces.ICurrentUserService>(sp => sp.GetRequiredService<PGB.Auth.Api.Services.CurrentUserService>());

// Register Application services (MediatR, validators, behaviors, AutoMapper)
builder.Services.AddApplicationServices(Assembly.Load("PGB.Auth.Application"));

// Register JwtTokenService from Infrastructure
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Register Auth DbContext
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
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddSingleton(SecuritySettings.Default());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use shared middleware (correlation id, etc)
// Dòng này sẽ chạy UserClaimsMiddleware của chúng ta để tạo HttpContext.User
app.UseWebApiCommon();

// --- PHẦN CẬP NHẬT: Thêm middleware Authentication và Authorization ---
// Quan trọng: Phải đặt sau UseWebApiCommon và trước MapControllers
app.UseAuthentication();
app.UseAuthorization();
// --- KẾT THÚC CẬP NHẬT ---

app.MapControllers();
app.Run();

// Make the implicit Program class available for integration tests
public partial class Program { }