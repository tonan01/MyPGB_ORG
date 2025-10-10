using PGB.BuildingBlocks.Application.Commands;
using PGB.BuildingBlocks.Application.Exceptions;
using PGB.Auth.Application.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PGB.Auth.Application.Commands.Handlers
{
    public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
    {
        #region Dependencies
        private readonly IUserRepository _userRepository;
        #endregion

        #region Constructor
        public DeleteUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        #endregion

        #region Handle Method
        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            // Ngăn không cho admin tự xóa chính mình
            if (request.Id == request.UserId)
            {
                throw new ApplicationValidationException("You cannot delete your own account.");
            }

            var userToDelete = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

            if (userToDelete == null)
            {
                throw new NotFoundException("User", request.Id);
            }

            // Thay vì xóa cứng, chúng ta sẽ gọi phương thức soft-delete (nếu có)
            // Trong BaseEntity đã có sẵn MarkAsDeleted
            userToDelete.MarkAsDeleted($"admin_{request.UserId}");

            // Không cần gọi _userRepository.DeleteAsync()
            await _userRepository.SaveChangesAsync(cancellationToken);
        } 
        #endregion
    }
}