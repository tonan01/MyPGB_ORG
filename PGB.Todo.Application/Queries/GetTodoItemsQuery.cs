using PGB.BuildingBlocks.Application.Models; // Thêm using này
using PGB.BuildingBlocks.Application.Queries;
using PGB.Todo.Application.Dtos;

namespace PGB.Todo.Application.Queries
{
    // THAY ĐỔI: Kế thừa từ PagedQuery thay vì BaseQuery
    public class GetTodoItemsQuery : PagedQuery<TodoItemDto>
    {
        // UserId đã có sẵn trong BaseQuery, không cần thêm gì ở đây
    }
}