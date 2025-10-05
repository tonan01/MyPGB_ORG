using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PGB.Auth.Application.Services;
using PGB.Auth.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic; // Thêm using này
using System.Linq; // Thêm using này

namespace PGB.Auth.Infrastructure.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        // ... (Constructor và các dependencies giữ nguyên) ...
        #region Dependencies
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtTokenService> _logger;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationMinutes;
        #endregion

        #region Constructor
        public JwtTokenService(IConfiguration configuration, ILogger<JwtTokenService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _secretKey = configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            _issuer = configuration["JwtSettings:Issuer"] ?? "PGB_ORG";
            _audience = configuration["JwtSettings:Audience"] ?? "PGB_ORG_Users";
            _expirationMinutes = int.Parse(configuration["JwtSettings:ExpirationInMinutes"] ?? "60");
        }
        #endregion

        public JwtToken GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var expiresAt = DateTime.UtcNow.AddMinutes(_expirationMinutes);

            // --- PHẦN CẬP NHẬT: SỬ DỤNG TÊN CLAIM NGẮN GỌN ---
            var claims = new List<Claim>
    {
        new Claim("sub", user.Id.ToString()), // 'sub' (subject) là tiêu chuẩn cho User ID
        new Claim("name", user.UsernameValue), // 'name' thay cho ClaimTypes.Name
        new Claim("email", user.EmailValue), // 'email' thay cho ClaimTypes.Email
        new Claim("fullName", user.DisplayName),
        new Claim("isEmailVerified", user.IsEmailVerified.ToString()),
        new Claim("jti", Guid.NewGuid().ToString())
    };

            if (user.UserRoles != null && user.UserRoles.Any())
            {
                var roleNames = user.UserRoles.Select(ur => ur.Role.Name);
                foreach (var roleName in roleNames)
                {
                    claims.Add(new Claim("role", roleName)); // 'role' thay cho ClaimTypes.Role
                }
            }
            // --- KẾT THÚC CẬP NHẬT ---

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresAt,
                Issuer = _issuer,
                Audience = _audience,
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

        // ... (Các phương thức khác giữ nguyên)
        #region Implementation
        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretKey);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "ValidateToken failed");
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