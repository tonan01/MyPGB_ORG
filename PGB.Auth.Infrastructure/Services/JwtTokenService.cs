using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PGB.Auth.Application.Services;
using PGB.Auth.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace PGB.Auth.Infrastructure.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        #region Dependencies
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<JwtTokenService> _logger;
        #endregion

        #region Constructor
        public JwtTokenService(IOptions<JwtSettings> jwtSettings, ILogger<JwtTokenService> logger)
        {
            // Lấy đối tượng cấu hình từ IOptions
            _jwtSettings = jwtSettings.Value;
            _logger = logger;

            // Kiểm tra cấu hình ngay từ đầu để tránh lỗi runtime
            if (string.IsNullOrEmpty(_jwtSettings.SecretKey))
            {
                throw new InvalidOperationException("JWT SecretKey not configured in appsettings.json");
            }
            if (_jwtSettings.ExpirationInMinutes <= 0)
            {
                _jwtSettings.ExpirationInMinutes = 60; // Gán giá trị mặc định nếu chưa cấu hình
            }
        }
        #endregion

        #region Generate Token
        public JwtToken GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UsernameValue),
                new Claim(ClaimTypes.Email, user.EmailValue),
                new Claim("fullName", user.DisplayName),
                new Claim("isEmailVerified", user.IsEmailVerified.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (user.UserRoles != null && user.UserRoles.Any())
            {
                var roleNames = user.UserRoles.Select(ur => ur.Role.Name);
                foreach (var roleName in roleNames)
                {
                    claims.Add(new Claim(ClaimTypes.Role, roleName));
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresAt,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return new JwtToken
            {
                Token = tokenString,
                ExpiresAt = expiresAt,
                TokenType = "Bearer"
            };
        }
        #endregion

        #region Validate Token
        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Token validation failed.");
                return null;
            }
        }

        public Guid? GetUserIdFromToken(string token)
        {
            var principal = ValidateToken(token);
            var userIdClaim = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
        #endregion
    }
}