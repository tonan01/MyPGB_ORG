using MediatR;
using PGB.BuildingBlocks.Application.Commands;
using PGB.BuildingBlocks.Application.Exceptions;
using PGB.Todo.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace PGB.Todo.Application.Commands.Handlers
{
    public class DeleteTodoItemCommandHandler : ICommandHandler<DeleteTodoItemCommand>
    {
        #region Fields
        private readonly ITodoRepository _todoRepository;
        #endregion

        #region Constructor
        public DeleteTodoItemCommandHandler(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }
        #endregion

        #region Methods
        public async Task Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
        {
            var todoItem = await _todoRepository.GetByIdAsync(request.Id);

            if (todoItem == null)
            {
                throw new NotFoundException("TodoItem", request.Id);
            }

            // Rất quan trọng: Kiểm tra quyền sở hữu
            if (todoItem.UserId != request.UserId)
            {
                throw new AuthorizationException("You are not authorized to delete this item.");
            }

            _todoRepository.Delete(todoItem);
        } 
        #endregion
    }
}