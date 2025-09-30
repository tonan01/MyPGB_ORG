using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using PGB.BuildingBlocks.WebApi.Common.Filters;
using PGB.BuildingBlocks.WebApi.Common.Middleware;

namespace PGB.BuildingBlocks.WebApi.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebApiCommon(this IServiceCollection services)
        {
            services.AddScoped<GlobalExceptionFilter>();
            services.AddMemoryCache();
            services.Configure<PGB.BuildingBlocks.WebApi.Common.Models.RateLimitOptions>(config => { });
            // RedisRateLimitStore is registered by the host if Redis is enabled; default is in-memory limiter.
            return services;
        }

        public static WebApplicationBuilder AddWebApiCommon(this WebApplicationBuilder builder)
        {
            builder.Services.AddWebApiCommon();
            // bind RateLimitOptions from configuration section "RateLimiting" if present
            var section = builder.Configuration.GetSection("RateLimiting");
            if (section != null && section.Value != null)
            {
                builder.Services.Configure<PGB.BuildingBlocks.WebApi.Common.Models.RateLimitOptions>(section);
            }
            return builder;
        }
    }
}


