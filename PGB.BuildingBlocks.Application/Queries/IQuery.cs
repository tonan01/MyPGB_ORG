using MediatR;

namespace PGB.BuildingBlocks.Application.Queries
{
    #region Query Interfaces
    public interface IQuery<out TResponse> : IRequest<TResponse>
    {
    } 
    #endregion
}