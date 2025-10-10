using System;

namespace PGB.BuildingBlocks.Domain.Interfaces
{
    public interface ICurrentUserService
    {
        #region Methods
        string? GetCurrentUsername(); 
        #endregion
    }
}
