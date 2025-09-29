using MediatR;
using PGB.BuildingBlocks.Application.Commands;
using PGB.BuildingBlocks.Domain.ValueObjects;
using PGB.Auth.Domain.Entities;
using PGB.Auth.Domain.ValueObjects;
using PGB.Auth.Application.Services;
using PGB.Auth.Application.Repositories;
using PGB.BuildingBlocks.Application.Exceptions;

namespace PGB.Auth.Application.Commands.Handlers
{
    #region Register User Command Handler
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
            
            // 4. Save to repository
            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);
            
            // 5. Return response
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
        #endregion

        #region Private Methods
        private async Task ValidateInput(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Password confirmation
            if (request.Password != request.ConfirmPassword)
                throw new ValidationException("Mật khẩu xác nhận không khớp");
            
            // Check username uniqueness
            if (!await _userDomainService.IsUsernameAvailableAsync(request.Username, cancellationToken))
                throw new ValidationException("Username đã được sử dụng");
            
            // Check email uniqueness
            if (!await _userDomainService.IsEmailAvailableAsync(request.Email, cancellationToken))
                throw new ValidationException("Email đã được sử dụng");
            
            // Password strength
            if (!_userDomainService.IsPasswordStrong(request.Password))
                throw new ValidationException("Mật khẩu không đủ mạnh");
        }
        #endregion
    }
    #endregion
}