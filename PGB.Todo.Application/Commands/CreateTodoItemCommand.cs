using MediatR;
using PGB.BuildingBlocks.Application.Commands;
using PGB.Todo.Application.Dtos;
using PGB.Todo.Domain.Entities;
using System;

namespace PGB.Todo.Application.Commands
{
    public class CreateTodoItemCommand : BaseCommand<TodoItemDto>
    {
        #region Properties
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public Priority Priority { get; set; } = Priority.Medium; 
        #endregion
    }
}