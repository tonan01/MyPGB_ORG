using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using PGB.Auth.Application.Repositories;
using PGB.Auth.Application.Services;
using PGB.Auth.Domain.ValueObjects;
using PGB.Auth.Infrastructure;
using PGB.Auth.Infrastructure.Data;
using PGB.Auth.Infrastructure.Repositories;
using PGB.Auth.Infrastructure.Services;
using PGB.BuildingBlocks.Application.Behaviors;
using PGB.BuildingBlocks.Domain.Interfaces;
using PGB.BuildingBlocks.WebApi.Common.Extensions;
using PGB.BuildingBlocks.WebApi.Common.Filters;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký GlobalExceptionFilter & Thêm cấu hình IOptions
builder.Services.AddControllers(options => options.Filters.Add<GlobalExceptionFilter>());
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));


// Add services
builder.AddWebApiCommon();
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
builder.Services.AddScoped<ICurrentUserService>(sp => sp.GetRequiredService<PGB.Auth.Api.Services.CurrentUserService>());

// MediatR và AutoMapper
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("PGB.Auth.Application")));
builder.Services.AddAutoMapper(Assembly.Load("PGB.Auth.Application"));

// Đăng ký các Pipeline Behavior
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserDomainService, UserDomainService>();
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddSingleton(SecuritySettings.Default());

// Đăng ký DbContext
var conn = builder.Configuration.GetConnectionString("DefaultConnection")
          ?? throw new InvalidOperationException("DB connection string 'DefaultConnection' not configured");
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(conn, sql => sql.MigrationsAssembly("PGB.Auth.Infrastructure")));
builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<AuthDbContext>());


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

#region Database Initialization
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<AuthDbContext>();
        await DbInitializer.InitializeAsync(context, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}
#endregion

app.UseWebApiCommon();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }