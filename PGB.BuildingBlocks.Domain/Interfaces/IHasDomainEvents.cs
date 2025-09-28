using PGB.BuildingBlocks.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.BuildingBlocks.Domain.Interfaces
{
    public interface IHasDomainEvents
    {
        #region Properties
        IReadOnlyList<IDomainEvent> DomainEvents { get; }
        #endregion

        #region Methods
        void ClearDomainEvents(); 
        #endregion
    }
}
