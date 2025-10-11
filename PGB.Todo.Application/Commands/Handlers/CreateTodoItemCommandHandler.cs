using AutoMapper;
using PGB.BuildingBlocks.Application.Commands;
using PGB.Todo.Application.Dtos;
using PGB.Todo.Application.Interfaces;
using PGB.Todo.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace PGB.Todo.Application.Commands.Handlers
{
    public class CreateTodoItemCommandHandler : ICommandHandler<CreateTodoItemCommand, TodoItemDto>
    {
        private readonly ITodoRepository _todoRepository;
        private readonly IMapper _mapper;

        public CreateTodoItemCommandHandler(ITodoRepository todoRepository, IMapper mapper)
        {
            _todoRepository = todoRepository;
            _mapper = mapper;
        }

        public async Task<TodoItemDto> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
        {
            var todoItem = TodoItem.Create(
                request.Title,
                request.Description,
                request.UserId,
                request.DueDate,
                request.Priority);

            await _todoRepository.AddAsync(todoItem);

            return _mapper.Map<TodoItemDto>(todoItem);
        }
    }
}