using MediatR;
using PGB.Auth.Application.Repositories;
using PGB.Auth.Application.Services;
using PGB.Auth.Domain.Entities;
using PGB.Auth.Domain.ValueObjects;
using PGB.BuildingBlocks.Application.Commands;
using PGB.BuildingBlocks.Application.Exceptions;
using PGB.BuildingBlocks.Domain.Exceptions;
using IPasswordHasher = PGB.Auth.Domain.ValueObjects.IPasswordHasher;

namespace PGB.Auth.Application.Commands.Handlers
{
    #region Login User Command Handler
    public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, LoginUserResponse>
    {
        #region Dependencies
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUserDomainService _userDomainService;
        private readonly SecuritySettings _securitySettings;
        #endregion

        #region Constructor
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
        #endregion

        #region Handle Method
        public async Task<LoginUserResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            // 1. Find user by username or email
            var user = await FindUserByUsernameOrEmail(request.UsernameOrEmail, cancellationToken);

            if (user == null)
            {
                // Record failed attempt even if user not found (security)
                throw new AuthenticationException("Thông tin đăng nhập không hợp lệ");
            }

            // 2. Verify password
            if (!user.VerifyPassword(request.Password, _passwordHasher))
            {
                user.RecordFailedLogin(_securitySettings, "system");
                await _userRepository.SaveChangesAsync(cancellationToken);
                throw new AuthenticationException("Thông tin đăng nhập không hợp lệ");
            }

            // Proceed with login and targeted retry on concurrency
            for (int attempt = 0; attempt < 2; attempt++)
            {
                try
                {
                    // 3. Successful login
                    user.Login(request.IpAddress ?? "Unknown", request.UserAgent ?? "Unknown", "system");

                    // 4. Generate tokens
                    var accessToken = _jwtTokenService.GenerateAccessToken(user);
                    var refreshTokenValue = _userDomainService.GenerateRefreshToken();
                    var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(_securitySettings.RefreshTokenLifetimeDays);

                    var refreshToken = user.AddRefreshToken(refreshTokenValue, refreshTokenExpiresAt, "system");

                    // 5. Save changes
                    await _userRepository.SaveChangesAsync(cancellationToken);

                    // 6. Return response
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
                catch (PGB.BuildingBlocks.Application.Exceptions.ConcurrencyException)
                {
                    if (attempt == 0)
                    {
                        // refresh user from DB and retry once
                        user = await FindUserByUsernameOrEmail(request.UsernameOrEmail, cancellationToken);
                        if (user == null)
                            throw new AuthenticationException("Thông tin đăng nhập không hợp lệ");
                        continue; // retry
                    }

                    // second failure -> propagate concurrency as application exception
                    throw;
                }
            }

            // Should not reach here
            throw new AuthenticationException("Thông tin đăng nhập không hợp lệ");
        }
        #endregion

        #region Private Methods
        private async Task<User?> FindUserByUsernameOrEmail(string usernameOrEmail, CancellationToken cancellationToken)
        {
            // Try as username first
            var user = await _userRepository.GetByUsernameAsync(usernameOrEmail, cancellationToken);

            if (user == null)
            {
                // Try as email
                user = await _userRepository.GetByEmailAsync(usernameOrEmail, cancellationToken);
            }

            return user;
        }
        #endregion
    }
    #endregion
}