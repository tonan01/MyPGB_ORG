using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace PGB.Auth.Infrastructure.Data
{
    public class DesignTimeAuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
    {
        public AuthDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();

            // Try to read connection string from env var, otherwise fallback to localdb for design-time
            // Prefer environment variable AUTH_DEFAULT_CONNECTION, otherwise fallback to provided default
            var conn = Environment.GetEnvironmentVariable("AUTH_DEFAULT_CONNECTION") ?? "Server=localhost;Database=PGB_AuthDb;User Id=sa;Password=123;TrustServerCertificate=true;MultipleActiveResultSets=true";

            optionsBuilder.UseSqlServer(conn, sql =>
            {
                sql.MigrationsAssembly(typeof(AuthDbContext).Assembly.FullName);
            });

            return new AuthDbContext(optionsBuilder.Options);
        }
    }
}


