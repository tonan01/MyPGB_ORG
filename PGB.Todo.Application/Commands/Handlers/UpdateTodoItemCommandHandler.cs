using MediatR;
using PGB.BuildingBlocks.Application.Commands;
using PGB.BuildingBlocks.Application.Exceptions;
using PGB.Todo.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace PGB.Todo.Application.Commands.Handlers
{
    public class UpdateTodoItemCommandHandler : ICommandHandler<UpdateTodoItemCommand>
    {
        private readonly ITodoRepository _todoRepository;

        public UpdateTodoItemCommandHandler(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }

        public async Task Handle(UpdateTodoItemCommand request, CancellationToken cancellationToken)
        {
            var todoItem = await _todoRepository.GetByIdAsync(request.Id);

            if (todoItem == null)
            {
                throw new NotFoundException("TodoItem", request.Id);
            }

            if (todoItem.UserId != request.UserId)
            {
                throw new AuthorizationException("You are not authorized to update this item.");
            }

            todoItem.Update(request.Title, request.Description, request.DueDate, request.Priority);

            if (request.IsCompleted)
            {
                todoItem.Complete();
            }
            else
            {
                todoItem.Uncomplete();
            }

            _todoRepository.Update(todoItem);
        }
    }
}