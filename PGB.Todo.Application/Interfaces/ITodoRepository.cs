using PGB.BuildingBlocks.Application.Models; // Thêm using này
using PGB.Todo.Application.Queries; // Thêm using này
using PGB.Todo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PGB.Todo.Application.Interfaces
{
    public interface ITodoRepository
    {
        Task<TodoItem?> GetByIdAsync(Guid id);
        // Giữ lại phương thức cũ nếu cần, hoặc xóa đi và thêm phương thức mới
        Task<IEnumerable<TodoItem>> GetByUserIdAsync(Guid userId);

        // --- THÊM PHƯƠNG THỨC MỚI CHO PHÂN TRANG ---
        Task<PagedResult<TodoItem>> GetPagedByUserIdAsync(GetTodoItemsQuery query);

        Task AddAsync(TodoItem todoItem);
        void Update(TodoItem todoItem);
        void Delete(TodoItem todoItem);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}