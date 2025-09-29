using Microsoft.Extensions.DependencyInjection;
using MediatR;
using FluentValidation;
using System.Reflection;
using PGB.BuildingBlocks.Application.Behaviors;

namespace PGB.BuildingBlocks.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        #region Add Application Services
        public static IServiceCollection AddApplicationServices(
          this IServiceCollection services,
          params Assembly[] assemblies)
        {
            // MediatR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(assemblies);
            });

            // FluentValidation
            services.AddValidatorsFromAssemblies(assemblies);

            // Pipeline behaviors
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

            // AutoMapper
            services.AddAutoMapper(assemblies);

            return services;
        } 
        #endregion
    }
}