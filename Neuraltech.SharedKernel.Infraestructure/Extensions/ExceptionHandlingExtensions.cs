using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Neuraltech.SharedKernel.Infraestructure.ExceptionHandlers;
using Neuraltech.SharedKernel.Infraestructure.Filters;

namespace Neuraltech.SharedKernel.Infraestructure.Extensions
{
    public static class ExceptionHandlingExtensions
    {
        public static IServiceCollection UseExceptionHandlers(this IServiceCollection services)
        {
            services.AddProblemDetails(configure =>
            {
                configure.CustomizeProblemDetails = context =>
                {
                    context.ProblemDetails.Extensions.TryAdd(
                        "traceId",
                        context.HttpContext.TraceIdentifier
                    );
                };
            });

            services.AddExceptionHandler<IdAlreadyInDbExceptionHandler>();
            services.AddExceptionHandler<EntityToUpdateNotFoundExceptionHandler>();
            services.AddExceptionHandler<EntityToDeleteNotFoundExceptionHandler>();
            services.AddExceptionHandler<GlobalExceptionHandler>();

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add<ControllerTypeFilter>();
            });

            return services;
        }
    }
}
