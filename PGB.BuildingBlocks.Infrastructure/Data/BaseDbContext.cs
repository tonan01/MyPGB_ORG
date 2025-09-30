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

        protected BaseDbContext(DbContextOptions options) : base(options)
        {
        }

        protected BaseDbContext(DbContextOptions options, IMediator mediator, ICurrentUserService currentUserService) : base(options)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        public override int SaveChanges()
        {
            UpdateAuditableEntities();
            var result = base.SaveChanges();
            DispatchDomainEvents().GetAwaiter().GetResult();
            return result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditableEntities();
            var result = await base.SaveChangesAsync(cancellationToken);
            await DispatchDomainEvents();
            return result;
        }

        private void UpdateAuditableEntities()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity<Guid>)
                .ToList();

            foreach (var entry in entries)
            {
                var entity = (BaseEntity<Guid>)entry.Entity;
                switch (entry.State)
                {
                    case EntityState.Added:
                        // use EF property accessors to set protected setters
                        entry.Property(nameof(BaseEntity<Guid>.CreatedAt)).CurrentValue = DateTime.UtcNow;
                        entry.Property(nameof(BaseEntity<Guid>.CreatedBy)).CurrentValue = _currentUserService?.GetCurrentUsername() ?? "system";
                        break;
                    case EntityState.Modified:
                        entity.MarkAsUpdated(_currentUserService?.GetCurrentUsername() ?? "system");
                        break;
                }
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

            // Global query filter for soft delete
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity<Guid>).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var body = Expression.Equal(
                        Expression.Property(parameter, nameof(BaseEntity<Guid>.IsDeleted)),
                        Expression.Constant(false));
                    var lambda = Expression.Lambda(body, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }

            // Configure audit fields for all BaseEntity types
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity<Guid>).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType, b =>
                    {
                        b.Property(nameof(BaseEntity<Guid>.CreatedBy)).HasMaxLength(100);
                        b.Property(nameof(BaseEntity<Guid>.UpdatedBy)).HasMaxLength(100);
                        b.Property(nameof(BaseEntity<Guid>.DeletedBy)).HasMaxLength(100);

                        // Ignore DomainEvents for EF
                        //b.Ignore(nameof(BaseEntity<Guid>.DomainEvents));
                    });
                }
            }
        }
    }
}
