using MediatR;

namespace PGB.BuildingBlocks.Application.Commands
{
    #region Command Interfaces
    public abstract class BaseCommand : ICommand
    {
        #region Common Properties
        public Guid UserId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        #endregion
    }
    #endregion

    #region Command Classes
    public abstract class BaseCommand<TResponse> : ICommand<TResponse>
    {
        #region Common Properties
        public Guid UserId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        #endregion
    } 
    #endregion
}