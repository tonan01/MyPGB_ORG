using PGB.Auth.Application.Services;
using PGB.Auth.Application.Repositories;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace PGB.Auth.Infrastructure.Services
{
    #region User Domain Service Implementation
    public class UserDomainService : IUserDomainService
    {
        #region Dependencies
        private readonly IUserRepository _userRepository;
        #endregion

        #region Constructor
        public UserDomainService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        #endregion

        #region Implementation
        public async Task<bool> IsUsernameAvailableAsync(string username, CancellationToken cancellationToken = default)
        {
            return !await _userRepository.ExistsByUsernameAsync(username, cancellationToken);
        }

        public async Task<bool> IsEmailAvailableAsync(string email, CancellationToken cancellationToken = default)
        {
            return !await _userRepository.ExistsByEmailAsync(email, cancellationToken);
        }

        public bool IsPasswordStrong(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            // At least 8 characters
            if (password.Length < 8)
                return false;

            // Must contain at least 3 of 4 character types:
            // - Lowercase letter
            // - Uppercase letter  
            // - Digit
            // - Special character
            var hasLower = Regex.IsMatch(password, @"[a-z]");
            var hasUpper = Regex.IsMatch(password, @"[A-Z]");
            var hasDigit = Regex.IsMatch(password, @"\d");
            var hasSpecial = Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]");

            var criteriaMet = (hasLower ? 1 : 0) + (hasUpper ? 1 : 0) + (hasDigit ? 1 : 0) + (hasSpecial ? 1 : 0);

            return criteriaMet >= 3;
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
        #endregion
    }
    #endregion
}