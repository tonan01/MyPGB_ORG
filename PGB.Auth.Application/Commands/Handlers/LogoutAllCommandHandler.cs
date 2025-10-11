using MediatR;
using PGB.BuildingBlocks.Application.Commands;
using PGB.BuildingBlocks.Application.Exceptions;
using PGB.Auth.Application.Repositories;

namespace PGB.Auth.Application.Commands.Handlers
{
    public class LogoutAllCommandHandler : ICommandHandler<LogoutAllCommand>
    {
        private readonly IUserRepository _userRepository;

        public LogoutAllCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Handle(LogoutAllCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null)
                throw new NotFoundException("User không tồn tại");

            user.RevokeAllRefreshTokens("User logout from all devices");
        }
    }
}