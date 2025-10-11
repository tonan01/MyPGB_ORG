using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace PGB.Todo.Infrastructure.Persistence
{
    public class DesignTimeTodoDbContextFactory : IDesignTimeDbContextFactory<TodoDbContext>
    {
        #region Methods
        public TodoDbContext CreateDbContext(string[] args)
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            string path = Directory.GetCurrentDirectory();
            var apiPath = Path.GetFullPath(Path.Combine(path, @"..\PGB.Todo.Api"));

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(apiPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .Build();

            var builder = new DbContextOptionsBuilder<TodoDbContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidDataException($"Could not find a connection string named 'DefaultConnection' in appsettings.{environment}.json");
            }

            builder.UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsAssembly(typeof(TodoDbContext).Assembly.FullName);
            });

            return new TodoDbContext(builder.Options); 
            #endregion
        }
    }
}