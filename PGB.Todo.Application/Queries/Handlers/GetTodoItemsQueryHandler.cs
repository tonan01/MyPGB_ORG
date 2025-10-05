using AutoMapper;
using PGB.BuildingBlocks.Application.Models; // Thêm
using PGB.BuildingBlocks.Application.Queries;
using PGB.Todo.Application.Dtos;
using PGB.Todo.Application.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PGB.Todo.Application.Queries.Handlers
{
    // --- THAY ĐỔI KIỂU TRẢ VỀ ---
    public class GetTodoItemsQueryHandler : IQueryHandler<GetTodoItemsQuery, PagedResult<TodoItemDto>>
    {
        private readonly ITodoRepository _todoRepository;
        private readonly IMapper _mapper;

        public GetTodoItemsQueryHandler(ITodoRepository todoRepository, IMapper mapper)
        {
            _todoRepository = todoRepository;
            _mapper = mapper;
        }

        // --- CẬP NHẬT LOGIC ---
        public async Task<PagedResult<TodoItemDto>> Handle(GetTodoItemsQuery request, CancellationToken cancellationToken)
        {
            // Gọi phương thức phân trang mới
            var pagedResult = await _todoRepository.GetPagedByUserIdAsync(request);

            // Map danh sách items sang DTO
            var todoItemDtos = _mapper.Map<List<TodoItemDto>>(pagedResult.Items);

            // Trả về kết quả phân trang với dữ liệu DTO
            return new PagedResult<TodoItemDto>(todoItemDtos, pagedResult.TotalCount, pagedResult.Page, pagedResult.PageSize);
        }
    }
}