using AutoMapper;
using PGB.BuildingBlocks.Application.Queries;
using PGB.Todo.Application.Dtos;
using PGB.Todo.Application.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PGB.Todo.Application.Queries.Handlers
{
    public class GetTodoItemsQueryHandler : IQueryHandler<GetTodoItemsQuery, IEnumerable<TodoItemDto>>
    {
        private readonly ITodoRepository _todoRepository;
        private readonly IMapper _mapper;

        public GetTodoItemsQueryHandler(ITodoRepository todoRepository, IMapper mapper)
        {
            _todoRepository = todoRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TodoItemDto>> Handle(GetTodoItemsQuery request, CancellationToken cancellationToken)
        {
            var todoItems = await _todoRepository.GetByUserIdAsync(request.UserId);
            return _mapper.Map<IEnumerable<TodoItemDto>>(todoItems);
        }
    }
}