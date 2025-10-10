using PGB.BuildingBlocks.Application.Models;
using PGB.BuildingBlocks.Application.Queries;
using PGB.Todo.Application.Dtos;

namespace PGB.Todo.Application.Queries
{
    public class GetTodoItemsQuery : PagedQuery<TodoItemDto>
    {
        // UserId đã có sẵn trong BaseQuery, không cần thêm gì ở đây
    }
}