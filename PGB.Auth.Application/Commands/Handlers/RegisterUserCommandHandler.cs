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
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserDomainService _userDomainService;
        private readonly SecuritySettings _securitySettings;

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

        public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            await ValidateInput(request, cancellationToken);

            var username = Username.Create(request.Username);
            var email = Email.Create(request.Email);
            var fullName = FullName.Create(request.FirstName, request.LastName);
            var hashedPassword = HashedPassword.Create(request.Password, _passwordHasher);

            var user = User.Register(username, email, fullName, hashedPassword);

            var defaultRole = await _userRepository.GetRoleByNameAsync(AppRoles.User, cancellationToken);
            if (defaultRole == null)
            {
                throw new InvalidOperationException($"Default role '{AppRoles.User}' not found in the database. Please seed the roles.");
            }
            user.AddRole(defaultRole);

            await _userRepository.AddAsync(user, cancellationToken);

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
    }
}