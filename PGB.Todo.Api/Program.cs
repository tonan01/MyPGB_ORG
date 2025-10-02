using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using PGB.BuildingBlocks.Application.Extensions;
using PGB.BuildingBlocks.WebApi.Common.Extensions;
using PGB.Todo.Application.Interfaces;
using PGB.Todo.Infrastructure.Persistence;
using PGB.Todo.Infrastructure.Repositories;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.AddWebApiCommon();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cấu hình Authentication & Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer();
builder.Services.AddAuthorization();

// Đăng ký các dịch vụ của tầng Application (MediatR, AutoMapper)
builder.Services.AddApplicationServices(Assembly.Load("PGB.Todo.Application"));

// Đăng ký DbContext
var conn = builder.Configuration.GetConnectionString("DefaultConnection")
          ?? throw new InvalidOperationException("DB connection string 'DefaultConnection' not configured");
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseSqlServer(conn, sql =>
    {
        sql.MigrationsAssembly("PGB.Todo.Infrastructure");
    }));

// Đăng ký Repository
builder.Services.AddScoped<ITodoRepository, TodoRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Thêm các middleware
app.UseWebApiCommon();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();