using MediatR;
using PGB.BuildingBlocks.Application.Commands;
using System;

namespace PGB.Todo.Application.Commands
{
    public class DeleteTodoItemCommand : BaseCommand
    {
        // Id của công việc cần xóa, lấy từ URL
        #region Properties
        public Guid Id { get; set; } 
        #endregion
    }
}