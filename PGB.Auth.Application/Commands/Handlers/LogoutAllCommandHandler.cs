using MediatR;
using PGB.BuildingBlocks.Application.Commands;
using PGB.BuildingBlocks.Application.Exceptions;
using PGB.Auth.Application.Repositories;

namespace PGB.Auth.Application.Commands.Handlers
{
    #region Logout All Command Handler
    public class LogoutAllCommandHandler : ICommandHandler<LogoutAllCommand>
    {
        #region Dependencies
        private readonly IUserRepository _userRepository;
        #endregion

        #region Constructor
        public LogoutAllCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        #endregion

        #region Handle Method
        public async Task Handle(LogoutAllCommand request, CancellationToken cancellationToken)
        {
            // 1. Find user
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null)
                throw new NotFoundException("User không tồn tại");

            // 2. Revoke all refresh tokens
            user.RevokeAllRefreshTokens("system", "User logout from all devices");
        }
        #endregion
    }
    #endregion
}