using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace PGB.BuildingBlocks.WebApi.Common.Middleware
{
    /// <summary>
    /// Middleware to construct a ClaimsPrincipal from headers forwarded by the API Gateway.
    /// This allows downstream services to use standard [Authorize] attributes.
    /// </summary>
    public class UserClaimsMiddleware
    {
        private readonly RequestDelegate _next;

        public UserClaimsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check for the User-Id header forwarded by the gateway
            if (context.Request.Headers.TryGetValue("X-User-Id", out var userIdValue) &&
                !string.IsNullOrEmpty(userIdValue))
            {
                // Create a list of claims, starting with the user's ID
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userIdValue.ToString())
                };

                // Check for the User-Roles header and add role claims
                if (context.Request.Headers.TryGetValue("X-User-Roles", out var userRolesValue) &&
                    !string.IsNullOrEmpty(userRolesValue))
                {
                    var roles = userRolesValue.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var role in roles)
                    {
                        if (!string.IsNullOrWhiteSpace(role))
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role.Trim()));
                        }
                    }
                }

                // Create a new ClaimsIdentity and replace the existing HttpContext.User
                var identity = new ClaimsIdentity(claims, "GatewayAuthentication");
                context.User = new ClaimsPrincipal(identity);
            }

            await _next(context);
        }
    }
}