using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Neuraltech.SharedKernel.Domain.Contracts;
using Neuraltech.SharedKernel.Infraestructure.Services;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neuraltech.SharedKernel.Infraestructure.Extensions
{
    public static class BaseExtensions
    {
        public static IHostApplicationBuilder UseFluentValidation<T>(
           this IHostApplicationBuilder builder
        )
        {
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<T>();

            return builder;
        }

        public static IHostApplicationBuilder UseLogging(this IHostApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();

            builder.Logging.AddConsole();

            return builder;
        }

        public static IServiceCollection UseTimeProvider(
           this IServiceCollection services
        )
        {
            services.AddSingleton(TimeProvider.System);
            return services;
        }

        public static IServiceCollection UseSleeper(
            this IServiceCollection services
        )
        {
            services.AddScoped<ISleeper, Sleeper>();
            return services;
        }
    }
}
