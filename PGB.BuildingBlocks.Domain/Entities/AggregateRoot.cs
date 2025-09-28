using PGB.BuildingBlocks.Domain.Events;
using PGB.BuildingBlocks.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.BuildingBlocks.Domain.Entities
{
    public abstract class AggregateRoot<TId> : BaseEntity<TId>, IHasDomainEvents
    {
        #region Domain Events
        private readonly List<IDomainEvent> _domainEvents = new();

        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
        #endregion

        #region Constructors
        protected AggregateRoot() : base() { }

        protected AggregateRoot(TId id) : base(id) { }
        #endregion

        #region Domain Event Methods
        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        protected void RemoveDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        protected void RaiseDomainEvent(IDomainEvent domainEvent)
        {
            AddDomainEvent(domainEvent);
        }
        #endregion
    }

    #region Convenience Classes - Guid-based Aggregate
    public abstract class AggregateRoot : AggregateRoot<Guid>
    {
        protected AggregateRoot() : base(Guid.NewGuid()) { }
        protected AggregateRoot(Guid id) : base(id) { }
    }
    #endregion
}
