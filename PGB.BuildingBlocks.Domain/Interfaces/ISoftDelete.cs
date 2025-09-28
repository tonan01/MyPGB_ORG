using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.BuildingBlocks.Domain.Interfaces
{
    public interface ISoftDelete
    {
        #region Properties
        bool IsDeleted { get; }
        DateTime? DeletedAt { get; }
        string? DeletedBy { get; }
        #endregion

        #region Methods
        void MarkAsDeleted(string deletedBy);
        void Restore(string restoredBy); 
        #endregion
    }
}
