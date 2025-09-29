using PGB.BuildingBlocks.Application.Commands;

namespace PGB.Auth.Application.Commands
{
    #region Command
    public class LoginUserCommand : BaseCommand<LoginUserResponse>
    {
        #region Properties
        public string UsernameOrEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public bool RememberMe { get; set; } = false; 
        #endregion
    } 
    #endregion

    #region Response
    public class LoginUserResponse
    {
        #region Properties
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiresAt { get; set; }
        public DateTime RefreshTokenExpiresAt { get; set; }

        // User info
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool IsEmailVerified { get; set; } 
        #endregion
    } 
    #endregion
}