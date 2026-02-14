using Grafana.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Neuraltech.SharedKernel.Infraestructure.Extensions
{
    public static class ObservabilityExtension
    {
        public static IHostApplicationBuilder UseObservability(
            this IHostApplicationBuilder builder,
            string serviceName
        )
        {
            builder.Logging.ClearProviders();

            builder.Logging.AddConsole();
            builder.Logging.AddOpenTelemetry(options =>
            {
                options.IncludeScopes = true;
                options.IncludeFormattedMessage = true;
            });

            var otel = builder.Services.AddOpenTelemetry();

            otel.ConfigureResource(resource => resource.AddService(serviceName))
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddNpgsqlInstrumentation()
                        .AddFusionCacheInstrumentation()
                        .AddMeter("Wolverine");
                })
                .WithTracing(tracing =>
                {
                    tracing
                        .AddSource(serviceName)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddEntityFrameworkCoreInstrumentation()
                        .AddRedisInstrumentation()
                        .AddNpgsql()
                        .AddFusionCacheInstrumentation()
                        .AddSource("Wolverine");
                });

            // Export OpenTelemetry data via OTLP, using env vars for the configuration
            var OtlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
            if (OtlpEndpoint != null)
            {
                otel.UseOtlpExporter();
            }

            return builder;
        }
    }
}
