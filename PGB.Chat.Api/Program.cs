using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using PGB.BuildingBlocks.Application.Extensions;
using PGB.BuildingBlocks.WebApi.Common.Extensions;
using PGB.Chat.Application.Interfaces;
using PGB.Chat.Infrastructure.Persistence;
using PGB.Chat.Infrastructure.Repositories;
using PGB.Chat.Infrastructure.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.AddWebApiCommon();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient(); // Đăng ký IHttpClientFactory

// Cấu hình Authentication & Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer();
builder.Services.AddAuthorization();

// Đăng ký các dịch vụ của tầng Application
builder.Services.AddApplicationServices(Assembly.Load("PGB.Chat.Application"));

// Đăng ký DbContext
var conn = builder.Configuration.GetConnectionString("DefaultConnection")
          ?? throw new InvalidOperationException("DB connection string 'DefaultConnection' not configured");
builder.Services.AddDbContext<ChatDbContext>(options =>
    options.UseSqlServer(conn, sql =>
    {
        sql.MigrationsAssembly("PGB.Chat.Infrastructure");
    }));

// Đăng ký các dịch vụ của tầng Infrastructure
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IAiChatService, OpenAiChatService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseWebApiCommon();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();