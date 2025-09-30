using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace PGB.BuildingBlocks.WebApi.Common.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.ContainsKey("X-Correlation-Id"))
            {
                context.Request.Headers["X-Correlation-Id"] = context.TraceIdentifier;
            }

            context.Response.Headers["X-Correlation-Id"] = context.Request.Headers["X-Correlation-Id"].ToString();

            await _next(context);
        }
    }
}


