using PGB.BuildingBlocks.Application.Models;
using PGB.Todo.Application.Queries;
using PGB.Todo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PGB.Todo.Application.Interfaces
{
    public interface ITodoRepository
    {
        #region Methods
        Task<TodoItem?> GetByIdAsync(Guid id);
        Task<IEnumerable<TodoItem>> GetByUserIdAsync(Guid userId);

        Task<PagedResult<TodoItem>> GetPagedByUserIdAsync(GetTodoItemsQuery query);

        Task AddAsync(TodoItem todoItem);
        void Update(TodoItem todoItem);
        void Delete(TodoItem todoItem);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken); 
        #endregion
    }
}