using Microsoft.EntityFrameworkCore;
using PGB.BuildingBlocks.Application.Models;
using PGB.Todo.Application.Interfaces;
using PGB.Todo.Application.Queries;
using PGB.Todo.Domain.Entities;
using PGB.Todo.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PGB.Todo.Infrastructure.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        private readonly TodoDbContext _context;

        public TodoRepository(TodoDbContext context)
        {
            _context = context;
        }

        public async Task<TodoItem?> GetByIdAsync(Guid id)
        {
            return await _context.TodoItems.FindAsync(id);
        }

        public async Task<IEnumerable<TodoItem>> GetByUserIdAsync(Guid userId)
        {
            return await _context.TodoItems
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(TodoItem todoItem)
        {
            await _context.TodoItems.AddAsync(todoItem);
        }

        // === BẮT ĐẦU SỬA LỖI ===
        public void Update(TodoItem todoItem)
        {
            // KHÔNG CẦN LÀM GÌ CẢ.
            // EF Core's Change Tracker sẽ tự động phát hiện các thay đổi 
            // khi các thuộc tính của 'todoItem' được sửa trong Command Handler.
        }

        public void Delete(TodoItem todoItem)
        {
            // THAY VÌ XÓA CỨNG, CHÚNG TA THỰC HIỆN XÓA MỀM
            // BaseDbContext sẽ tự điền 'DeletedBy' và 'DeletedAt'
            todoItem.MarkAsDeleted(string.Empty);
        }
        // === KẾT THÚC SỬA LỖI ===

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<PagedResult<TodoItem>> GetPagedByUserIdAsync(GetTodoItemsQuery query)
        {
            query.ValidateAndNormalize();

            var queryable = _context.TodoItems
                                    .Where(t => t.UserId == query.UserId)
                                    .OrderByDescending(t => t.CreatedAt);

            var totalCount = await queryable.CountAsync();

            var items = await queryable
                                .Skip((query.Page - 1) * query.PageSize)
                                .Take(query.PageSize)
                                .ToListAsync();

            return new PagedResult<TodoItem>(items, totalCount, query.Page, query.PageSize);
        }
    }
}