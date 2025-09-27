using PGB.BuildingBlocks.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.Todo.Domain.Events
{
    public class TodoItemCompletedEvent : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredAt { get; } = DateTime.UtcNow;

        public Guid TodoItemId { get; }
        public string Title { get; }
        public Guid UserId { get; }

        public TodoItemCompletedEvent(Guid todoItemId, string title, Guid userId)
        {
            TodoItemId = todoItemId;
            Title = title;
            UserId = userId;
        }
    }
}
