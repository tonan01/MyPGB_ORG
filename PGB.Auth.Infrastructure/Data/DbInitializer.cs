using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PGB.Auth.Domain.Entities;
using PGB.Auth.Infrastructure.Data;
using PGB.BuildingBlocks.Domain.Common;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PGB.Auth.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(AuthDbContext context, ILogger logger)
        {
            try
            {
                // 1. Áp dụng các migration đang chờ xử lý (nếu có)
                if (context.Database.GetPendingMigrations().Any())
                {
                    logger.LogInformation("Applying database migrations...");
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Database migrations applied successfully.");
                }

                // 2. Gieo mầm (Seed) dữ liệu cho Roles
                await SeedRolesAsync(context, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }

        private static async Task SeedRolesAsync(AuthDbContext context, ILogger logger)
        {
            // Chỉ seed khi bảng Roles chưa có dữ liệu
            if (!await context.Roles.AnyAsync())
            {
                logger.LogInformation("Seeding default roles...");
                await context.Roles.AddRangeAsync(
                    new Role(AppRoles.Admin, "Administrator role with full permissions."),
                    new Role(AppRoles.Manager, "Manager role with elevated permissions."),
                    new Role(AppRoles.User, "Standard user role.")
                );
                await context.SaveChangesAsync();
                logger.LogInformation("Default roles seeded successfully.");
            }
        }
    }
}