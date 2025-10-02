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

            // Rất quan trọng: Kiểm tra xem người dùng có phải là chủ sở hữu của công việc này không
            if (todoItem.UserId != request.UserId)
            {
                throw new AuthorizationException("You are not authorized to update this item.");
            }

            // Cập nhật thông tin
            todoItem.Update(request.Title, request.Description, request.DueDate, request.Priority, "system");

            if (request.IsCompleted)
            {
                todoItem.Complete("system");
            }
            else
            {
                todoItem.Uncomplete("system");
            }

            _todoRepository.Update(todoItem);
            await _todoRepository.SaveChangesAsync(cancellationToken);
        }
    }
}