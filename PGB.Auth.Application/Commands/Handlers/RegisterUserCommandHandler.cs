using MediatR;
using PGB.BuildingBlocks.Application.Commands;
using PGB.BuildingBlocks.Domain.ValueObjects;
using PGB.Auth.Domain.Entities;
using PGB.Auth.Domain.ValueObjects;
using PGB.Auth.Application.Services;
using PGB.Auth.Application.Repositories;
using PGB.BuildingBlocks.Application.Exceptions;
using PGB.BuildingBlocks.Domain.Common;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace PGB.Auth.Application.Commands.Handlers
{
    public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, RegisterUserResponse>
    {
        #region Dependencies
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserDomainService _userDomainService;
        private readonly SecuritySettings _securitySettings;
        #endregion

        #region Constructor
        public RegisterUserCommandHandler(
          IUserRepository userRepository,
          IPasswordHasher passwordHasher,
          IUserDomainService userDomainService,
          SecuritySettings securitySettings)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _userDomainService = userDomainService;
            _securitySettings = securitySettings;
        }
        #endregion

        #region Handle Method
        public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate input
            await ValidateInput(request, cancellationToken);

            // 2. Create Value Objects
            var username = Username.Create(request.Username);
            var email = Email.Create(request.Email);
            var fullName = FullName.Create(request.FirstName, request.LastName);
            var hashedPassword = HashedPassword.Create(request.Password, _passwordHasher);

            // 3. Create User Aggregate
            var user = User.Register(username, email, fullName, hashedPassword, "system");

            // 4. Assign default role
            var defaultRole = await _userRepository.GetRoleByNameAsync(AppRoles.User, cancellationToken);
            if (defaultRole == null)
            {
                throw new InvalidOperationException($"Default role '{AppRoles.User}' not found in the database. Please seed the roles.");
            }
            user.AddRole(defaultRole, "system");

            // 5. Save to repository
            await _userRepository.AddAsync(user, cancellationToken);

            // 6. Return response
            return new RegisterUserResponse
            {
                UserId = user.Id,
                Username = user.UsernameValue,
                Email = user.EmailValue,
                FullName = user.DisplayName,
                RequireEmailVerification = _securitySettings.RequireEmailVerification,
                CreatedAt = user.CreatedAt
            };
        }

        private async Task ValidateInput(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (request.Password != request.ConfirmPassword)
                throw new ValidationException("Mật khẩu xác nhận không khớp");

            if (!await _userDomainService.IsUsernameAvailableAsync(request.Username, cancellationToken))
                throw new ValidationException("Username đã được sử dụng");

            if (!await _userDomainService.IsEmailAvailableAsync(request.Email, cancellationToken))
                throw new ValidationException("Email đã được sử dụng");

            if (!_userDomainService.IsPasswordStrong(request.Password))
                throw new ValidationException("Mật khẩu không đủ mạnh");
        } 
        #endregion
    }
}