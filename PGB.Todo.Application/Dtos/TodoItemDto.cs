using PGB.Todo.Domain.Enums;
using System;

namespace PGB.Todo.Application.Dtos
{
    public class TodoItemDto
    {
        #region Properties
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime? DueDate { get; set; }
        public Priority Priority { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; } 
        #endregion
    }
}