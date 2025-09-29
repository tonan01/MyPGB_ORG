using MediatR;

namespace PGB.BuildingBlocks.Application.Commands
{
    #region Command Interfaces
    public interface ICommand : IRequest
    {
    }

    public interface ICommand<out TResponse> : IRequest<TResponse>
    {
    } 
    #endregion
}