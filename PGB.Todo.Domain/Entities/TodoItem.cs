using PGB.BuildingBlocks.Domain.Entities;
using PGB.Todo.Domain.Events;
using PGB.Todo.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.Todo.Domain.Entities
{
    public class TodoItem : AggregateRoot
    {
        #region Properties
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public bool IsCompleted { get; private set; } = false;
        public DateTime? DueDate { get; private set; }
        public Priority Priority { get; private set; } = Priority.Medium;
        public Guid UserId { get; private set; }
        #endregion

        #region Constructor
        protected TodoItem() { }

        public static TodoItem Create(string title, string description, Guid userId,
            DateTime? dueDate = null, Priority priority = Priority.Medium)
        {
            var todoItem = new TodoItem
            {
                Title = title,
                Description = description,
                UserId = userId,
                DueDate = dueDate,
                Priority = priority
            };

            todoItem.AddDomainEvent(new TodoItemCreatedEvent(todoItem.Id, title, userId));

            return todoItem;
        }
        #endregion

        #region Methods
        public void Update(string title, string description, DateTime? dueDate, Priority priority)
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
        }

        public void Complete()
        {
            IsCompleted = true;
            AddDomainEvent(new TodoItemCompletedEvent(Id, Title, UserId));
        }

        public void Uncomplete()
        {
            IsCompleted = false;
        }
        #endregion
    }
}