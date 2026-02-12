using Confluent.Kafka;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace Neuraltech.SharedKernel.Infraestructure.Extensions
{
    public static class HealthCheckExtensions
    {
        /// <summary>
        /// https://aspire.dev/fundamentals/health-checks/
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHealthChecksBuilder UseHealthChecks(
            this IHostApplicationBuilder builder
        )
        {
            var healthChecksRequestTimeout = builder.Configuration.GetValue("HealthChecks:TimeoutSeconds", 5L);
            builder.Services.AddRequestTimeouts(
               timeouts =>
               {
                   timeouts.AddPolicy("HealthChecks", TimeSpan.FromSeconds(healthChecksRequestTimeout));
               }
            );

            var healthChecksCacheTimeout = builder.Configuration.GetValue("HealthChecks:TimeoutSeconds", 5L);
            builder.Services.AddOutputCache(
                caching =>
                {
                    caching.AddPolicy(
                        "HealthChecks", 
                        policy => policy.Expire(TimeSpan.FromSeconds(healthChecksCacheTimeout))
                    );
                }
            );

            builder.Services.AddHealthChecks()
                            // Add a default liveness check to ensure app is responsive
                            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

            return builder.Services.AddHealthChecks();
        }

        
        public static IHealthChecksBuilder AddRedis(
          this IHealthChecksBuilder builder,
          IConfiguration configuration
        )
        {
            var redisConnStr = configuration.GetConnectionString("Redis");
            builder.AddRedis(redisConnStr!);

            return builder;
        }

        public static IHealthChecksBuilder AddKafka(
          this IHealthChecksBuilder builder,
          IConfiguration configuration
        )
        {
            var cfg = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                AllowAutoCreateTopics = true
            };

            if (configuration.GetSection("Kafka:Auth").Exists())
            {
                cfg.SaslUsername = configuration["Kafka:Auth:Username"];
                cfg.SaslPassword = configuration["Kafka:Auth:Password"];
                cfg.SaslMechanism = Enum.Parse<SaslMechanism>(configuration["Kafka:Auth:SaslMechanism"]!);
                cfg.SecurityProtocol = Enum.Parse<SecurityProtocol>(configuration["Kafka:Auth:SecurityProtocol"]!);
            }

            builder.AddKafka(cfg);
            return builder;
        }

        public static WebApplication MapHealthCheckEndpoints(this WebApplication app)
        {

            var healthChecks = app.MapGroup("");

            healthChecks
                .CacheOutput("HealthChecks")
                .WithRequestTimeout("HealthChecks");

            // All health checks must pass for app to be
            // considered ready to accept traffic after starting
            healthChecks.MapHealthChecks(
                "/health",
                new HealthCheckOptions
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                }
            );

            // Only health checks tagged with the "live" tag
            // must pass for app to be considered alive
            healthChecks.MapHealthChecks("/alive", new()
            {
                Predicate = static r => r.Tags.Contains("live")
            });

            return app;
        }
    }
}
