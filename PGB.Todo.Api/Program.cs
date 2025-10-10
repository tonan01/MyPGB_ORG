using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using PGB.BuildingBlocks.Application.Extensions;
using PGB.BuildingBlocks.WebApi.Common.Extensions;
using PGB.Todo.Application.Interfaces;
using PGB.Todo.Infrastructure.Persistence;
using PGB.Todo.Infrastructure.Repositories;
using System.Reflection;
// THÊM USING CHO SERVICE MỚI
using PGB.Todo.Api.Services;
using PGB.BuildingBlocks.Domain.Interfaces;
using Microsoft.Extensions.Logging;

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

// Authentication & Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer();
builder.Services.AddAuthorization();

// --- BẮT ĐẦU: Đăng ký ICurrentUserService ---
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddScoped<ICurrentUserService>(sp => sp.GetRequiredService<CurrentUserService>());
// --- KẾT THÚC: Đăng ký ICurrentUserService ---

// Register Application services
builder.Services.AddApplicationServices(Assembly.Load("PGB.Todo.Application"));

// Register DbContext
var conn = builder.Configuration.GetConnectionString("DefaultConnection")
          ?? throw new InvalidOperationException("DB connection string 'DefaultConnection' not configured");
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseNpgsql(conn, sql =>
    {
        sql.MigrationsAssembly("PGB.Todo.Infrastructure");
    }));

// Register Repository
builder.Services.AddScoped<ITodoRepository, TodoRepository>();

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

// --- BẮT ĐẦU: Code tự động Apply Migrations (Giải quyết lỗi 42P01) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<TodoDbContext>();
        logger.LogInformation("Applying database migrations for TodoDbContext...");
        await context.Database.MigrateAsync();
        logger.LogInformation("TodoDbContext migrations applied successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while applying migrations for TodoDbContext.");
    }
}
// --- KẾT THÚC: Code tự động Apply Migrations ---

// Add middlewares
app.UseWebApiCommon();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();