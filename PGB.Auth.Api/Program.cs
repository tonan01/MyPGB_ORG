using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using PGB.Auth.Application.Repositories;
using PGB.Auth.Application.Services;
using PGB.Auth.Domain.ValueObjects;
using PGB.Auth.Infrastructure.Data;
using PGB.Auth.Infrastructure.Repositories;
using PGB.Auth.Infrastructure.Services;
using PGB.BuildingBlocks.Application.Extensions;
using PGB.BuildingBlocks.WebApi.Common.Extensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.AddWebApiCommon();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerGen();

// Authentication and Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer();

builder.Services.AddAuthorization();

// Register services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<PGB.Auth.Api.Services.CurrentUserService>();
builder.Services.AddScoped<PGB.BuildingBlocks.Domain.Interfaces.ICurrentUserService>(sp => sp.GetRequiredService<PGB.Auth.Api.Services.CurrentUserService>());
builder.Services.AddApplicationServices(Assembly.Load("PGB.Auth.Application"));
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserDomainService, UserDomainService>();
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddSingleton(SecuritySettings.Default());

// Register DbContext
var conn = builder.Configuration.GetConnectionString("DefaultConnection")
          ?? throw new InvalidOperationException("DB connection string 'DefaultConnection' not configured");
builder.Services.AddDbContextPool<AuthDbContext>(options =>
    options.UseNpgsql(conn, sql =>
    {
        sql.MigrationsAssembly("PGB.Auth.Infrastructure");
    }));

var app = builder.Build();

var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
    });
}

app.UseWebApiCommon();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// --- TỰ ĐỘNG APPLY MIGRATIONS KHI KHỞI ĐỘNG ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AuthDbContext>();
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}
// --- KẾT THÚC PHẦN CODE MỚI ---

app.Run();

public partial class Program { }