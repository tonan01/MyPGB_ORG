using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using PGB.BuildingBlocks.WebApi.Common.Filters;

namespace PGB.BuildingBlocks.WebApi.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebApiCommon(this IServiceCollection services)
        {
            services.AddScoped<GlobalExceptionFilter>();
            return services;
        }

        public static WebApplicationBuilder AddWebApiCommon(this WebApplicationBuilder builder)
        {
            builder.Services.AddWebApiCommon();
            return builder;
        }
    }
}


