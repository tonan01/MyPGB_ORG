using Microsoft.EntityFrameworkCore;
using PGB.BuildingBlocks.Domain.Entities;
using PGB.BuildingBlocks.Domain.Events;
using MediatR;
using System.Linq.Expressions;

namespace PGB.BuildingBlocks.Infrastructure.Data
{
    public abstract class BaseDbContext : DbContext
    {
        private readonly IMediator? _mediator;

        protected BaseDbContext(DbContextOptions options) : base(options)
        {
        }

        protected BaseDbContext(DbContextOptions options, IMediator mediator) : base(options)
        {
            _mediator = mediator;
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
            var entries = ChangeTracker.Entries<BaseEntity<Guid>>();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.GetType()
                            .GetProperty(nameof(BaseEntity<Guid>.CreatedAt))?
                            .SetValue(entry.Entity, DateTime.UtcNow);
                        break;

                    case EntityState.Modified:
                        entry.Entity.MarkAsUpdated("system"); // You can inject current user later
                        break;
                }
            }
        }

        private async Task DispatchDomainEvents()
        {
            if (_mediator == null) return;

            var domainEntities = ChangeTracker
                .Entries<BaseEntity<Guid>>()
                .Where(x => x.Entity.DomainEvents.Any())
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent);
            }
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
                        b.Ignore(nameof(BaseEntity<Guid>.DomainEvents));
                    });
                }
            }
        }
    }
}
