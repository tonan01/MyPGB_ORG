using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace PGB.Todo.Infrastructure.Persistence
{
    public class DesignTimeTodoDbContextFactory : IDesignTimeDbContextFactory<TodoDbContext>
    {
        public TodoDbContext CreateDbContext(string[] args)
        {
            string path = Directory.GetCurrentDirectory();
            var apiPath = Path.GetFullPath(Path.Combine(path, @"..\PGB.Todo.Api"));

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(apiPath)
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<TodoDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidDataException("Could not find a connection string named 'DefaultConnection'.");
            }

            builder.UseNpgsql(connectionString);

            return new TodoDbContext(builder.Options);
        }
    }
}