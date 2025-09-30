using System;

namespace PGB.BuildingBlocks.Domain.Interfaces
{
    public interface ICurrentUserService
    {
        string? GetCurrentUsername();
    }
}
