using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.BuildingBlocks.Domain.Interfaces
{
    public interface IAuditable
    {
        #region Properties
        DateTime CreatedAt { get; }
        DateTime? UpdatedAt { get; }
        string CreatedBy { get; }
        string? UpdatedBy { get; } 
        #endregion
    }
}
