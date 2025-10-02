using PGB.BuildingBlocks.Application.Commands;
using System;

namespace PGB.Auth.Application.Commands
{
    public class DeleteUserCommand : BaseCommand
    {
        public Guid Id { get; set; }

        // UserId của người thực hiện hành động xóa (Admin) sẽ được lấy từ BaseCommand
    }
}