using MediatR;
using PGB.BuildingBlocks.Application.Commands;
using System;

namespace PGB.Todo.Application.Commands
{
    // THAY ĐỔI: Kế thừa từ BaseCommand thay vì ICommand
    public class DeleteTodoItemCommand : BaseCommand
    {
        // Id của công việc cần xóa, lấy từ URL
        public Guid Id { get; set; }

        // UserId đã có sẵn trong BaseCommand, sẽ được gán từ controller
    }
}