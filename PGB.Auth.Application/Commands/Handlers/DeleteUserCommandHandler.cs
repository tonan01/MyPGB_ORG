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
        private readonly IUserRepository _userRepository;

        public DeleteUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == request.UserId)
            {
                throw new ApplicationValidationException("You cannot delete your own account.");
            }

            var userToDelete = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

            if (userToDelete == null)
            {
                throw new NotFoundException("User", request.Id);
            }

            // BaseDbContext sẽ tự động gán DeletedBy
            userToDelete.MarkAsDeleted(string.Empty);
        }
    }
}