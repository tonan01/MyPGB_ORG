using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace PGB.BuildingBlocks.WebApi.Common.Middleware
{
    public class UserClaimsMiddleware
    {
        private readonly RequestDelegate _next;

        public UserClaimsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("X-User-Id", out var userIdValue) &&
                !string.IsNullOrEmpty(userIdValue))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userIdValue.ToString())
                };

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

                var identity = new ClaimsIdentity(claims, "GatewayAuthentication");
                context.User = new ClaimsPrincipal(identity);
            }

            await _next(context);
        }
    }
}