using PGB.Auth.Domain.Entities;
using System.Security.Claims;

namespace PGB.Auth.Application.Services
{
    #region IJwtTokenService
    public interface IJwtTokenService
    {
        #region Methods
        JwtToken GenerateAccessToken(User user);
        ClaimsPrincipal? ValidateToken(string token);
        Guid? GetUserIdFromToken(string token);
        #endregion
    }
    #endregion

    #region JWT Token Model
    public class JwtToken
    {
        #region Properties
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string TokenType { get; set; } = "Bearer";
        #endregion
    } 
    #endregion
}