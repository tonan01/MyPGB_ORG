using Microsoft.EntityFrameworkCore;
using PGB.Auth.Domain.Entities;
using PGB.BuildingBlocks.Infrastructure.Data;
using MediatR;

namespace PGB.Auth.Infrastructure.Data
{
    public class AuthDbContext : BaseDbContext
    {
        // ... (Constructor giữ nguyên) ...
        #region Constructor
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        internal AuthDbContext(DbContextOptions<AuthDbContext> options, IMediator mediator, PGB.BuildingBlocks.Domain.Interfaces.ICurrentUserService currentUserService)
            : base(options, mediator, currentUserService)
        {
        }
        #endregion

        #region DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        // --- PHẦN CẬP NHẬT ---
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        // --- KẾT THÚC CẬP NHẬT ---

        #endregion

        #region Model Configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Configuration (giữ nguyên)
            modelBuilder.Entity<User>(entity =>
            {
                // ... (giữ nguyên tất cả cấu hình của User)
                entity.HasKey(e => e.Id);

                // Username Value Object
                entity.OwnsOne(e => e.Username, username =>
                {
                    username.Property(u => u.Value)
                        .HasColumnName("Username")
                        .IsRequired()
                        .HasMaxLength(50);
                    // Create index on the owned value object's Value property
                    username.HasIndex(u => u.Value).IsUnique().HasDatabaseName("IX_Users_Username");
                });

                // Email Value Object
                entity.OwnsOne(e => e.Email, email =>
                {
                    email.Property(e => e.Value)
                        .HasColumnName("Email")
                        .IsRequired()
                        .HasMaxLength(255);
                    // Create index on the owned value object's Value property
                    email.HasIndex(e => e.Value).IsUnique().HasDatabaseName("IX_Users_Email");
                });

                // FullName Value Object
                entity.OwnsOne(e => e.FullName, fullName =>
                {
                    fullName.Property(fn => fn.FirstName)
                        .HasColumnName("FirstName")
                        .IsRequired()
                        .HasMaxLength(50);

                    fullName.Property(fn => fn.LastName)
                        .HasColumnName("LastName")
                        .IsRequired()
                        .HasMaxLength(50);

                    fullName.Property(fn => fn.MiddleName)
                        .HasColumnName("MiddleName")
                        .HasMaxLength(50);
                });

                // HashedPassword Value Object
                entity.OwnsOne(e => e.PasswordHash, password =>
                {
                    password.Property(p => p.Hash)
                        .HasColumnName("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(255);

                    password.Property(p => p.Algorithm)
                        .HasColumnName("PasswordAlgorithm")
                        .IsRequired()
                        .HasMaxLength(20);

                    password.Property(p => p.Salt)
                        .HasColumnName("PasswordSalt")
                        .HasMaxLength(255);

                    password.Property(p => p.CreatedAt)
                        .HasColumnName("PasswordCreatedAt");
                });

                // Other properties
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsEmailVerified).HasDefaultValue(false);
                entity.Property(e => e.FailedLoginAttempts).HasDefaultValue(0);

                // Indexes for performance handled inside owned type mappings above
                entity.HasIndex(e => e.IsActive).HasDatabaseName("IX_Users_IsActive");
                entity.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_Users_CreatedAt");
                entity.HasIndex(e => e.IsDeleted).HasDatabaseName("IX_Users_IsDeleted");
            });

            // RefreshToken Configuration (giữ nguyên)
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                // ... (giữ nguyên cấu hình của RefreshToken)
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Token).IsRequired().HasMaxLength(255);
                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(e => e.User)
                    .WithMany(e => e.RefreshTokens)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.Token).IsUnique().HasDatabaseName("IX_RefreshTokens_Token");
                entity.HasIndex(e => e.UserId).HasDatabaseName("IX_RefreshTokens_UserId");
                entity.HasIndex(e => e.ExpiresAt).HasDatabaseName("IX_RefreshTokens_ExpiresAt");
                entity.HasIndex(e => e.IsDeleted).HasDatabaseName("IX_RefreshTokens_IsDeleted");
            });

            // --- PHẦN CẬP NHẬT MỚI ---
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Name).IsRequired().HasMaxLength(50);
                entity.HasIndex(r => r.Name).IsUnique(); // Đảm bảo tên Role là duy nhất
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleId }); // Khóa chính kết hợp

                entity.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();

                entity.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
            });
            // --- KẾT THÚC CẬP NHẬT MỚI ---
        }
        #endregion
    }
}