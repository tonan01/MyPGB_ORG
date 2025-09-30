using Microsoft.AspNetCore.Builder;
using PGB.BuildingBlocks.WebApi.Common.Middleware;
using Microsoft.Extensions.DependencyInjection;

namespace PGB.BuildingBlocks.WebApi.Common.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseWebApiCommon(this IApplicationBuilder app)
        {
            app.UseMiddleware<CorrelationIdMiddleware>();
            return app;
        }

        public static WebApplication UseWebApiCommon(this WebApplication app)
        {
            app.UseMiddleware<CorrelationIdMiddleware>();
            // add rate limit middleware (requires IMemoryCache registered)
            var opts = app.Services.GetService<Microsoft.Extensions.Options.IOptions<PGB.BuildingBlocks.WebApi.Common.Models.RateLimitOptions>>()?.Value
                       ?? new PGB.BuildingBlocks.WebApi.Common.Models.RateLimitOptions();

            // If a ConnectionMultiplexer is registered, use Redis-backed limiter; otherwise fallback to in-memory
            var sp = app.Services;
            var mux = sp.GetService<StackExchange.Redis.IConnectionMultiplexer>();
            if (mux != null)
            {
                var store = sp.GetRequiredService<PGB.BuildingBlocks.WebApi.Common.Middleware.RedisRateLimitStore>();
                app.UseMiddleware<PGB.BuildingBlocks.WebApi.Common.Middleware.RedisRateLimitMiddleware>(store, opts);
            }
            else
            {
                var cache = sp.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>();
                app.UseMiddleware<PGB.BuildingBlocks.WebApi.Common.Middleware.RateLimitMiddleware>(cache, opts);
            }
            return app;
        }
    }
}


