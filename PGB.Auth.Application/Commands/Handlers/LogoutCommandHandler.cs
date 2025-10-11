using MediatR;
using PGB.BuildingBlocks.Application.Commands;
using PGB.BuildingBlocks.Application.Exceptions;
using PGB.Auth.Application.Repositories;

namespace PGB.Auth.Application.Commands.Handlers
{
    #region Logout Command Handler
    public class LogoutCommandHandler : ICommandHandler<LogoutCommand>
    {
        #region Dependencies
        private readonly IUserRepository _userRepository;
        #endregion

        #region Constructor
        public LogoutCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        #endregion

        #region Handle Method
        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            // 1. Find refresh token
            var refreshToken = await _userRepository.GetRefreshTokenAsync(
                request.RefreshToken,
                cancellationToken);

            if (refreshToken == null)
                throw new NotFoundException("Refresh token không tồn tại");

            // 2. Check if already revoked
            if (refreshToken.IsRevoked)
                return; // Already logged out, do nothing

            // 3. Check if expired
            if (refreshToken.IsExpired)
                throw new ApplicationValidationException("Refresh token đã hết hạn");

            // 4. Revoke the token
            refreshToken.Revoke("system", "User logout");
        }
        #endregion
    }
    #endregion
}