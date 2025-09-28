using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.BuildingBlocks.Domain.Events
{
    public abstract class IntegrationEvent : IDomainEvent
    {
        #region Properties
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public string EventType { get; }
        #endregion

        #region Constructors
        protected IntegrationEvent()
        {
            EventType = GetType().Name;
        } 
        #endregion
    }
}
