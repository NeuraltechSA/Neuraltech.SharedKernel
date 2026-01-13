using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace Neuraltech.SharedKernel.Infraestructure.Extensions
{
    public static class LoggingExtensions
    {
        public static IHostApplicationBuilder UseLogging(this IHostApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();

            builder.Logging.AddConsole();

            return builder;
        }
    }
}
