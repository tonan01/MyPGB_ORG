using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using PGB.BuildingBlocks.Application.Behaviors;
using PGB.BuildingBlocks.Domain.Interfaces;
using PGB.BuildingBlocks.WebApi.Common.Extensions;
using PGB.BuildingBlocks.WebApi.Common.Filters;
using PGB.Chat.Api.Services;
using PGB.Chat.Application.Interfaces;
using PGB.Chat.Infrastructure.Persistence;
using PGB.Chat.Infrastructure.Repositories;
using PGB.Chat.Infrastructure.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký GlobalExceptionFilter & Thêm cấu hình IOptions
builder.Services.AddControllers(options => options.Filters.Add<GlobalExceptionFilter>());
builder.Services.Configure<OpenAiSettings>(builder.Configuration.GetSection("OpenAI"));

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
builder.Services.AddHttpClient();

// Authentication & Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer();
builder.Services.AddAuthorization();

// Đăng ký ICurrentUserService
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddScoped<ICurrentUserService>(sp => sp.GetRequiredService<CurrentUserService>());

// MediatR và AutoMapper
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("PGB.Chat.Application")));
builder.Services.AddAutoMapper(Assembly.Load("PGB.Chat.Application"));

// Đăng ký các Pipeline Behavior
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));


// Đăng ký DbContext
var conn = builder.Configuration.GetConnectionString("DefaultConnection")
          ?? throw new InvalidOperationException("DB connection string 'DefaultConnection' not configured");
builder.Services.AddDbContext<ChatDbContext>(options =>
    options.UseNpgsql(conn, sql => sql.MigrationsAssembly("PGB.Chat.Infrastructure")));
builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<ChatDbContext>());


// Register Infrastructure services
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IAiChatService, OpenAiChatService>();

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

// Code tự động Apply Migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<ChatDbContext>();
        logger.LogInformation("Applying database migrations for ChatDbContext...");
        await context.Database.MigrateAsync();
        logger.LogInformation("ChatDbContext migrations applied successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while applying migrations for ChatDbContext.");
    }
}

app.UseWebApiCommon();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();