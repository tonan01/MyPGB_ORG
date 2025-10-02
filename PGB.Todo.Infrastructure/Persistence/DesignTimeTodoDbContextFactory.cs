using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace PGB.Todo.Infrastructure.Persistence
{
    /// <summary>
    /// This factory is used by the EF Core tools (e.g., for creating migrations) at design time.
    /// It creates a configured DbContext instance by reading the connection string from the Api project's appsettings.json.
    /// </summary>
    public class DesignTimeTodoDbContextFactory : IDesignTimeDbContextFactory<TodoDbContext>
    {
        public TodoDbContext CreateDbContext(string[] args)
        {
            // This logic helps the tool find the appsettings.json file from the Api project
            // when you are running commands from the Infrastructure project directory.
            string path = Directory.GetCurrentDirectory();

            // Navigate up to the solution directory and then down to the Api project
            // This makes the path more robust
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

            builder.UseSqlServer(connectionString);

            return new TodoDbContext(builder.Options);
        }
    }
}