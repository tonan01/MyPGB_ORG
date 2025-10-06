using MediatR;
using PGB.Auth.Application.Repositories;
using PGB.Auth.Application.Services;
using PGB.Auth.Domain.Entities;
using PGB.Auth.Domain.ValueObjects;
using PGB.BuildingBlocks.Application.Commands;
using PGB.BuildingBlocks.Application.Exceptions;
using PGB.BuildingBlocks.Domain.Exceptions;
using System.Threading;
using System.Threading.Tasks;
using System;
using IPasswordHasher = PGB.Auth.Domain.ValueObjects.IPasswordHasher;

namespace PGB.Auth.Application.Commands.Handlers
{
    public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, LoginUserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUserDomainService _userDomainService;
        private readonly SecuritySettings _securitySettings;

        public LoginUserCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwtTokenService,
            IUserDomainService userDomainService,
            SecuritySettings securitySettings)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _userDomainService = userDomainService;
            _securitySettings = securitySettings;
        }

        public async Task<LoginUserResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await FindUserByUsernameOrEmail(request.UsernameOrEmail, cancellationToken);

            if (user == null)
            {
                throw new AuthenticationException("Thông tin đăng nhập không hợp lệ");
            }

            if (!user.VerifyPassword(request.Password, _passwordHasher))
            {
                user.RecordFailedLogin(_securitySettings, "system");
                await _userRepository.SaveChangesAsync(cancellationToken);
                throw new AuthenticationException("Thông tin đăng nhập không hợp lệ");
            }

            // Logic đăng nhập và tạo token
            user.Login(request.IpAddress ?? "Unknown", request.UserAgent ?? "Unknown", "system");
            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshTokenValue = _userDomainService.GenerateRefreshToken();
            var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(_securitySettings.RefreshTokenLifetimeDays);
            var refreshToken = user.AddRefreshToken(refreshTokenValue, refreshTokenExpiresAt, "system");

            await _userRepository.UpdateAsync(user, cancellationToken);

            await _userRepository.SaveChangesAsync(cancellationToken);

            return new LoginUserResponse
            {
                AccessToken = accessToken.Token,
                RefreshToken = refreshToken.Token,
                AccessTokenExpiresAt = accessToken.ExpiresAt,
                RefreshTokenExpiresAt = refreshToken.ExpiresAt,
                UserId = user.Id,
                Username = user.UsernameValue,
                Email = user.EmailValue,
                FullName = user.DisplayName,
                IsEmailVerified = user.IsEmailVerified
            };
        }

        private async Task<User?> FindUserByUsernameOrEmail(string usernameOrEmail, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUsernameAsync(usernameOrEmail, cancellationToken);
            if (user == null)
            {
                user = await _userRepository.GetByEmailAsync(usernameOrEmail, cancellationToken);
            }
            return user;
        }
    }
}