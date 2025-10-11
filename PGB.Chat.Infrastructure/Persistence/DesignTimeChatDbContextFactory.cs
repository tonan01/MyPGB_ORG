using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace PGB.Chat.Infrastructure.Persistence
{
    public class DesignTimeChatDbContextFactory : IDesignTimeDbContextFactory<ChatDbContext>
    {
        #region Methods
        public ChatDbContext CreateDbContext(string[] args)
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            string path = Directory.GetCurrentDirectory();
            var apiPath = Path.GetFullPath(Path.Combine(path, @"..\PGB.Chat.Api"));

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(apiPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .Build();

            var builder = new DbContextOptionsBuilder<ChatDbContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidDataException($"Could not find a connection string named 'DefaultConnection' in appsettings.{environment}.json");
            }

            builder.UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsAssembly(typeof(ChatDbContext).Assembly.FullName);
            });

            return new ChatDbContext(builder.Options);
        } 
        #endregion
    }
}