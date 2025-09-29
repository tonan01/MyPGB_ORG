using PGB.BuildingBlocks.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.Todo.Domain.Events
{
    public class TodoItemCompletedEvent : DomainEvent
    {
        #region Properties
        public Guid TodoItemId { get; }
        public string Title { get; }
        public Guid UserId { get; }
        #endregion

        #region Constructors
        public TodoItemCompletedEvent(Guid todoItemId, string title, Guid userId)
        {
            TodoItemId = todoItemId;
            Title = title;
            UserId = userId;
        } 
        #endregion
    }
}
