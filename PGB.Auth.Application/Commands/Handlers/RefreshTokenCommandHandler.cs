using MediatR;
using PGB.Auth.Application.Repositories;
using PGB.Auth.Application.Services;
using PGB.Auth.Domain.ValueObjects;
using PGB.BuildingBlocks.Application.Commands;
using PGB.BuildingBlocks.Application.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PGB.Auth.Application.Commands.Handlers
{
    public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, RefreshTokenResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUserDomainService _userDomainService;
        private readonly SecuritySettings _securitySettings;

        public RefreshTokenCommandHandler(
            IUserRepository userRepository,
            IJwtTokenService jwtTokenService,
            IUserDomainService userDomainService,
            SecuritySettings securitySettings)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            _userDomainService = userDomainService;
            _securitySettings = securitySettings;
        }

        public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // 1. Tìm refresh token trong database
            var oldRefreshToken = await _userRepository.GetRefreshTokenAsync(request.RefreshToken, cancellationToken);

            if (oldRefreshToken == null || !oldRefreshToken.IsValid)
            {
                throw new ApplicationValidationException("Refresh token không hợp lệ hoặc đã hết hạn.");
            }

            var user = oldRefreshToken.User;

            // 2. Kiểm tra trạng thái của user (có bị khóa hay không)
            if (user == null || !user.IsActive || user.IsLocked)
            {
                throw new ApplicationValidationException("Tài khoản của bạn đã bị khóa hoặc không hoạt động.");
            }

            // 3. Tạo Access Token mới
            var newAccessToken = _jwtTokenService.GenerateAccessToken(user);

            // 4. Tạo Refresh Token mới và thu hồi token cũ
            var newRefreshTokenValue = _userDomainService.GenerateRefreshToken();
            var newRefreshTokenExpiresAt = DateTime.UtcNow.AddDays(_securitySettings.RefreshTokenLifetimeDays);

            // Đánh dấu token cũ đã được sử dụng
            oldRefreshToken.Use("system");

            // Thêm token mới cho user
            var newRefreshToken = user.AddRefreshToken(newRefreshTokenValue, newRefreshTokenExpiresAt, "system");

            // 5. Lưu tất cả thay đổi vào database
            await _userRepository.SaveChangesAsync(cancellationToken);

            // 6. Trả về cặp token mới
            return new RefreshTokenResponse
            {
                AccessToken = newAccessToken.Token,
                RefreshToken = newRefreshToken.Token,
                AccessTokenExpiresAt = newAccessToken.ExpiresAt,
                RefreshTokenExpiresAt = newRefreshToken.ExpiresAt
            };
        }
    }
}