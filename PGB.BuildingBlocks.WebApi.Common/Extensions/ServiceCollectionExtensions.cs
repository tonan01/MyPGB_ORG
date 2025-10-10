using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using PGB.BuildingBlocks.WebApi.Common.Filters;
using PGB.BuildingBlocks.WebApi.Common.Middleware;

namespace PGB.BuildingBlocks.WebApi.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        #region AddWebApiCommon
        public static IServiceCollection AddWebApiCommon(this IServiceCollection services)
        {
            services.AddScoped<GlobalExceptionFilter>();
            services.AddMemoryCache();
            // RateLimitOptions configuration removed per project request.
            // RedisRateLimitStore is registered by the host if Redis is enabled.
            return services;
        }
        #endregion

        #region AddWebApiCommon for WebApplicationBuilder
        public static WebApplicationBuilder AddWebApiCommon(this WebApplicationBuilder builder)
        {
            builder.Services.AddWebApiCommon();
            // Rate limiting configuration section removed per project request.
            return builder;
        } 
        #endregion
    }
}


