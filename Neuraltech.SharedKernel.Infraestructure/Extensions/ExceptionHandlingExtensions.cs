using Microsoft.Extensions.DependencyInjection;
using Neuraltech.SharedKernel.Infraestructure.ExceptionHandlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neuraltech.SharedKernel.Infraestructure.Extensions
{
    public static class ExceptionHandlingExtensions
    {
        public static IServiceCollection UseExceptionHandlers(
            this IServiceCollection services
        )
        {
            services.AddProblemDetails(configure =>
            {
                configure.CustomizeProblemDetails = context =>
                {
                    context.ProblemDetails.Extensions.TryAdd("traceId", context.HttpContext.TraceIdentifier);
                };
            });

            services.AddExceptionHandler<IdAlreadyInDbExceptionHandler>();
            services.AddExceptionHandler<GlobalExceptionHandler>();

            return services;
        }
    }
}
