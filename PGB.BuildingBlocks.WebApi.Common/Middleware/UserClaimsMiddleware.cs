using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging; // THÊM USING NÀY
using System;
using System.Collections.Generic;
using System.Linq; // THÊM USING NÀY
using System.Security.Claims;
using System.Threading.Tasks;

namespace PGB.BuildingBlocks.WebApi.Common.Middleware
{
    public class UserClaimsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UserClaimsMiddleware> _logger; // THÊM LOGGER

        // CẬP NHẬT CONSTRUCTOR
        public UserClaimsMiddleware(RequestDelegate next, ILogger<UserClaimsMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("--- UserClaimsMiddleware START ---");

            if (context.Request.Headers.TryGetValue("X-User-Id", out var userIdValue) &&
                !string.IsNullOrEmpty(userIdValue))
            {
                _logger.LogInformation("Found header X-User-Id: {UserId}", userIdValue);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userIdValue.ToString())
                };

                if (context.Request.Headers.TryGetValue("X-User-Roles", out var userRolesValue) &&
                    !string.IsNullOrEmpty(userRolesValue))
                {
                    _logger.LogInformation("Found header X-User-Roles: {UserRoles}", userRolesValue);

                    var roles = userRolesValue.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var role in roles)
                    {
                        if (!string.IsNullOrWhiteSpace(role))
                        {
                            var trimmedRole = role.Trim();
                            claims.Add(new Claim(ClaimTypes.Role, trimmedRole));
                            _logger.LogInformation("Added Role Claim: {Role}", trimmedRole);
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("Header X-User-Roles NOT FOUND.");
                }

                var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
                context.User = new ClaimsPrincipal(identity);

                var constructedRoles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value);
                _logger.LogInformation("Constructed HttpContext.User with Roles: [{Roles}]", string.Join(", ", constructedRoles));
            }
            else
            {
                _logger.LogWarning("Header X-User-Id NOT FOUND. Skipping claims creation.");
            }

            _logger.LogInformation("--- UserClaimsMiddleware END ---");

            await _next(context);
        }
    }
}