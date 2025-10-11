using Microsoft.EntityFrameworkCore;
using PGB.BuildingBlocks.Domain.Entities;
using MediatR;
using System.Linq;
using System.Linq.Expressions;
using PGB.BuildingBlocks.Domain.Interfaces;

namespace PGB.BuildingBlocks.Infrastructure.Data
{
    public abstract class BaseDbContext : DbContext
    {
        private readonly IMediator? _mediator;
        private readonly ICurrentUserService? _currentUserService;

        #region Constructors
        protected BaseDbContext(DbContextOptions options) : base(options)
        {
        }

        protected BaseDbContext(DbContextOptions options, IMediator mediator, ICurrentUserService currentUserService) : base(options)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }
        #endregion

        #region SaveChanges Overrides
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditableEntities();
            var result = await base.SaveChangesAsync(cancellationToken);
            await DispatchDomainEvents();
            return result;
        }
        #endregion

        #region Model Configuration

        private void UpdateAuditableEntities()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is IAuditable && (e.State == EntityState.Added || e.State == EntityState.Modified))
                .ToList();

            var currentUser = _currentUserService?.GetCurrentUsername() ?? "system";
            var now = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property(nameof(IAuditable.CreatedAt)).CurrentValue = now;
                    entry.Property(nameof(IAuditable.CreatedBy)).CurrentValue = currentUser;
                }

                // === BẮT ĐẦU SỬA LỖI ===
                // Logic này đảm bảo EF Core nhận biết được sự thay đổi để tạo lệnh UPDATE.
                if (entry.State == EntityState.Modified)
                {
                    entry.Property(nameof(IAuditable.UpdatedAt)).CurrentValue = now;
                    entry.Property(nameof(IAuditable.UpdatedBy)).CurrentValue = currentUser;

                    // Xử lý riêng cho việc xóa mềm
                    if (entry.Entity is ISoftDelete softDeleteEntity && softDeleteEntity.IsDeleted && entry.Property(nameof(ISoftDelete.IsDeleted)).IsModified)
                    {
                        entry.Property(nameof(ISoftDelete.DeletedAt)).CurrentValue = now;
                        entry.Property(nameof(ISoftDelete.DeletedBy)).CurrentValue = currentUser;
                    }
                }
                // === KẾT THÚC SỬA LỖI ===
            }
        }

        private async Task DispatchDomainEvents()
        {
            if (_mediator == null) return;

            var domainEntities = ChangeTracker
                .Entries()
                .Select(e => e.Entity)
                .OfType<IHasDomainEvents>()
                .Where(e => e.DomainEvents != null && e.DomainEvents.Any())
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(e => e.DomainEvents)
                .ToList();

            domainEntities.ForEach(e => e.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
                await _mediator.Publish(domainEvent);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Soft Delete Query Filter
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var body = Expression.Equal(
                        Expression.Property(parameter, nameof(ISoftDelete.IsDeleted)),
                        Expression.Constant(false));
                    var lambda = Expression.Lambda(body, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
            #endregion

            #region Audit Fields Configuration
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(IAuditable).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType, b =>
                    {
                        b.Property(nameof(IAuditable.CreatedBy)).HasMaxLength(100);
                        b.Property(nameof(IAuditable.UpdatedBy)).HasMaxLength(100);
                    });
                }
                if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType, b =>
                    {
                        b.Property(nameof(ISoftDelete.DeletedBy)).HasMaxLength(100);
                    });
                }
                if (typeof(BaseEntity<Guid>).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType, b =>
                    {
                        b.Property("RowVersion").IsRowVersion();
                    });
                }
            }
            #endregion
        }
        #endregion
    }
}