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
            app.UseMiddleware<UserClaimsMiddleware>(); // <-- TH�M D�NG N�Y
            return app;
        }

        public static WebApplication UseWebApiCommon(this WebApplication app)
        {
            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<UserClaimsMiddleware>(); // <-- TH�M D�NG N�Y
            return app;
        }
    }
}