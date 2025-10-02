using MediatR;
using PGB.BuildingBlocks.Application.Commands;
using PGB.Todo.Domain.Entities;
using System;

namespace PGB.Todo.Application.Commands
{
    // THAY ĐỔI: Kế thừa từ BaseCommand thay vì ICommand
    public class UpdateTodoItemCommand : BaseCommand
    {
        // Id của công việc cần sửa, lấy từ URL
        public Guid Id { get; set; }

        // Dữ liệu mới từ body request
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime? DueDate { get; set; }
        public Priority Priority { get; set; }

        // UserId đã có sẵn trong BaseCommand, sẽ được gán từ controller
    }
}