using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace PGB.BuildingBlocks.WebApi.Common.Middleware
{
    public class CorrelationIdMiddleware
    {
        #region Fields
        private readonly RequestDelegate _next; 
        #endregion

        #region Constructor
        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        #endregion

        #region InvokeAsync Method
        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.ContainsKey("X-Correlation-Id"))
            {
                context.Request.Headers["X-Correlation-Id"] = context.TraceIdentifier;
            }

            context.Response.Headers["X-Correlation-Id"] = context.Request.Headers["X-Correlation-Id"].ToString();

            await _next(context);
        } 
        #endregion
    }
}


