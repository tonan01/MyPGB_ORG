using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace PGB.Auth.Infrastructure.Data
{
    public class DesignTimeAuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
    {
        #region Methods
        public AuthDbContext CreateDbContext(string[] args)
        {
            // Lấy môi trường hiện tại, mặc định là "Development" khi chạy từ local
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            string path = Directory.GetCurrentDirectory();
            var apiPath = Path.GetFullPath(Path.Combine(path, @"..\PGB.Auth.Api"));

            // Xây dựng configuration để đọc cả 2 file appsettings
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(apiPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true) // <--- Thêm dòng này
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidDataException($"Could not find a connection string named 'DefaultConnection' in appsettings.{environment}.json");
            }

            optionsBuilder.UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsAssembly(typeof(AuthDbContext).Assembly.FullName);
            });

            return new AuthDbContext(optionsBuilder.Options);
        } 
        #endregion
    }
}