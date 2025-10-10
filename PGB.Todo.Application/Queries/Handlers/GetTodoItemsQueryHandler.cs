using AutoMapper;
using PGB.BuildingBlocks.Application.Models;
using PGB.BuildingBlocks.Application.Queries;
using PGB.Todo.Application.Dtos;
using PGB.Todo.Application.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PGB.Todo.Application.Queries.Handlers
{
    public class GetTodoItemsQueryHandler : IQueryHandler<GetTodoItemsQuery, PagedResult<TodoItemDto>>
    {
        #region Properties
        private readonly ITodoRepository _todoRepository;
        private readonly IMapper _mapper; 
        #endregion

        #region Constructor
        public GetTodoItemsQueryHandler(ITodoRepository todoRepository, IMapper mapper)
        {
            _todoRepository = todoRepository;
            _mapper = mapper;
        } 
        #endregion

        #region Methods
        public async Task<PagedResult<TodoItemDto>> Handle(GetTodoItemsQuery request, CancellationToken cancellationToken)
        {
            var pagedResult = await _todoRepository.GetPagedByUserIdAsync(request);

            var todoItemDtos = _mapper.Map<List<TodoItemDto>>(pagedResult.Items);

            // Trả về kết quả phân trang với dữ liệu DTO
            return new PagedResult<TodoItemDto>(todoItemDtos, pagedResult.TotalCount, pagedResult.Page, pagedResult.PageSize);
        } 
        #endregion
    }
}