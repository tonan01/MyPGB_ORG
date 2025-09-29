using MediatR;

namespace PGB.BuildingBlocks.Application.Commands
{
    #region Command Handler Interfaces
    public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
        where TCommand : BaseCommand
    {
    }

    public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : BaseCommand<TResponse>
    {
    }
    #endregion
}