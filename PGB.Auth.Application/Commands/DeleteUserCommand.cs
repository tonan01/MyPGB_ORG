using PGB.BuildingBlocks.Application.Commands;
using System;

namespace PGB.Auth.Application.Commands
{
    public class DeleteUserCommand : BaseCommand
    {
        #region Properties
        public Guid Id { get; set; } 
        #endregion

        // UserId của người thực hiện hành động xóa (Admin) sẽ được lấy từ BaseCommand
    }
}