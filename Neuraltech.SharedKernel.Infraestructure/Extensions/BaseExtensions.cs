using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Neuraltech.SharedKernel.Domain.Contracts;
using Neuraltech.SharedKernel.Domain.Services;
using Neuraltech.SharedKernel.Infraestructure.Services;

namespace Neuraltech.SharedKernel.Infraestructure.Extensions
{
    public static class BaseExtensions
    {


        /*
        public static IHostApplicationBuilder UseLogging(this IHostApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();

            builder.Logging.AddConsole();

            return builder;
        }*/

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

        public static IServiceCollection UseGuidGenerator(
            this IServiceCollection services
        )
        {
            services.AddScoped<IGuidGenerator, GuidGenerator>();
            return services;
        }

        public static IHostApplicationBuilder UseDefaultExtensions(
            this IHostApplicationBuilder builder
        )
        {
            builder.Services.AddRequestTimeouts();
            builder.Services.AddOutputCache();

            var serviceName = builder.Configuration.GetValue<string>("ServiceName");
            Ensure.NotNull(serviceName);

            builder.Services.AddServiceDiscovery();

            builder.UseObservability(serviceName!);

            builder.Services.UseTimeProvider();

            builder.Services.UseSleeper();

            builder.Services.UseGuidGenerator();


            return builder;
        }
    }
}
