using MediatR;

namespace PGB.BuildingBlocks.Application.Queries
{
    #region Query Handler Interfaces
    public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
        where TQuery : BaseQuery<TResponse>
    {
    } 
    #endregion
}