using Microsoft.AspNetCore.Http;
using PGB.BuildingBlocks.Domain.Interfaces;
using System.Linq;
using System.Security.Claims;

namespace PGB.Chat.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetCurrentUsername()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return null;

            if (httpContext.User?.Identity?.IsAuthenticated == true)
            {
                var name = httpContext.User.Identity?.Name;
                if (!string.IsNullOrEmpty(name)) return name;

                var nameId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                return nameId;
            }

            return null;
        }
    }
}