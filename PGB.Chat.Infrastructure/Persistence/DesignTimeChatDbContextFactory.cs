using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace PGB.Chat.Infrastructure.Persistence
{
    public class DesignTimeChatDbContextFactory : IDesignTimeDbContextFactory<ChatDbContext>
    {
        public ChatDbContext CreateDbContext(string[] args)
        {
            // Logic để tìm file appsettings.json từ project Api
            string path = Directory.GetCurrentDirectory();
            var apiPath = Path.GetFullPath(Path.Combine(path, @"..\PGB.Chat.Api"));

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(apiPath)
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<ChatDbContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidDataException("Could not find a connection string named 'DefaultConnection'.");
            }

            // Sử dụng Npgsql provider
            builder.UseNpgsql(connectionString, sql =>
            {
                sql.MigrationsAssembly(typeof(ChatDbContext).Assembly.FullName);
            });

            return new ChatDbContext(builder.Options);
        }
    }
}