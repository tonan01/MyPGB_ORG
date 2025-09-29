using MediatR;

namespace PGB.BuildingBlocks.Application.Queries
{
    #region Query Classes
    public abstract class BaseQuery<TResponse> : IQuery<TResponse>
    {
        #region Common Properties
        public Guid UserId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        #endregion
    } 
    #endregion
}