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

            #region Soft Delete Query Filter
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
            #endregion

            #region Audit Fields Configuration
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity<Guid>).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType, b =>
                    {
                        b.Property(nameof(BaseEntity<Guid>.CreatedBy)).HasMaxLength(100);
                        b.Property(nameof(BaseEntity<Guid>.UpdatedBy)).HasMaxLength(100);
                        b.Property(nameof(BaseEntity<Guid>.DeletedBy)).HasMaxLength(100);
                        // Configure RowVersion concurrency token if present
                        b.Property("RowVersion").IsRowVersion();
                    });
                }
            }
            #endregion
        } 
        #endregion
    }
}