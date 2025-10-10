using Microsoft.AspNetCore.Http;
using PGB.BuildingBlocks.Domain.Interfaces;

namespace PGB.Auth.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        #region Fields
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Constructor
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region Methods
        public string? GetCurrentUsername()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return null;

            if (httpContext.User?.Identity?.IsAuthenticated == true)
            {
                // prefer Name claim or NameIdentifier
                var name = httpContext.User.Identity?.Name;
                if (!string.IsNullOrEmpty(name)) return name;

                var nameId = httpContext.User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == "nameidentifier" || c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                return nameId;
            }

            return null;
        } 
        #endregion
    }
}


