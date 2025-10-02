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

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UsernameValue),
                new Claim(ClaimTypes.Email, user.EmailValue),
                new Claim("FullName", user.DisplayName),
                new Claim("IsEmailVerified", user.IsEmailVerified.ToString()),
                new Claim("jti", Guid.NewGuid().ToString())
            };

            // --- PHẦN CẬP NHẬT ---
            // Lấy danh sách tên Role từ navigation property và thêm vào claims
            // Cần đảm bảo User được truy vấn kèm theo UserRoles và Role
            if (user.UserRoles != null && user.UserRoles.Any())
            {
                var roleNames = user.UserRoles.Select(ur => ur.Role.Name);
                foreach (var roleName in roleNames)
                {
                    claims.Add(new Claim(ClaimTypes.Role, roleName));
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