using PGB.BuildingBlocks.Application.Commands;
using PGB.BuildingBlocks.Application.Exceptions;
using PGB.BuildingBlocks.Domain.ValueObjects;
using PGB.Auth.Application.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace PGB.Auth.Application.Commands.Handlers
{
    public class UpdateMyProfileCommandHandler : ICommandHandler<UpdateMyProfileCommand>
    {
        private readonly IUserRepository _userRepository;

        public UpdateMyProfileCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null)
            {
                throw new NotFoundException("User", request.UserId);
            }

            var newFullName = FullName.Create(request.FirstName, request.LastName);

            user.UpdateProfile(newFullName);
        }
    }
}