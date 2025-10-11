using MediatR;
using PGB.BuildingBlocks.Application.Commands;
using PGB.BuildingBlocks.Application.Exceptions;
using PGB.Auth.Application.Repositories;

namespace PGB.Auth.Application.Commands.Handlers
{
    public class LogoutCommandHandler : ICommandHandler<LogoutCommand>
    {
        private readonly IUserRepository _userRepository;

        public LogoutCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var refreshToken = await _userRepository.GetRefreshTokenAsync(
                request.RefreshToken,
                cancellationToken);

            if (refreshToken == null)
                throw new NotFoundException("Refresh token không tồn tại");

            if (refreshToken.IsRevoked)
                return;

            if (refreshToken.IsExpired)
                throw new ApplicationValidationException("Refresh token đã hết hạn");

            refreshToken.Revoke("User logout");
        }
    }
}