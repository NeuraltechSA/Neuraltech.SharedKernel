using EasyCaching.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Neuraltech.SharedKernel.Infraestructure.Extensions;

/// <summary>
/// Extension methods for configuring EasyCaching in the application.
/// Provides convenient methods to setup different cache providers.
/// </summary>
public static class EasyCachingExtensions
{
    /// <summary>
    /// Adds EasyCaching with InMemory provider.
    /// Useful for development, testing, or single-instance applications.
    /// </summary>
    /// <param name="builder">Host application builder</param>
    /// <param name="providerName">Name of the cache provider (default: "default")</param>
    /// <param name="maxSize">Maximum number of items in cache (default: 10000)</param>
    /// <param name="expirationScanFrequency">Frequency in seconds for expiration scans (default: 60)</param>
    /// <returns>Host application builder for chaining</returns>
    public static IHostApplicationBuilder UseEasyCachingInMemory(
        this IHostApplicationBuilder builder,
        string providerName = "default",
        int maxSize = 10000,
        int expirationScanFrequency = 60)
    {
        builder.Services.AddEasyCaching(options =>
        {
            options.UseInMemory(config =>
            {
                config.DBConfig = new InMemoryCachingOptions
                {
                    SizeLimit = maxSize,
                    ExpirationScanFrequency = expirationScanFrequency
                };
            }, providerName);
        });

        return builder;
    }

    /// <summary>
    /// Adds EasyCaching with Redis provider.
    /// Recommended for production environments with distributed cache requirements.
    /// </summary>
    /// <param name="builder">Host application builder</param>
    /// <param name="providerName">Name of the cache provider (default: "default")</param>
    /// <param name="host">Redis host (default: from configuration or "localhost")</param>
    /// <param name="port">Redis port (default: from configuration or 6379)</param>
    /// <param name="database">Redis database (default: from configuration or 0)</param>
    /// <param name="password">Redis password (default: from configuration or null)</param>
    /// <returns>Host application builder for chaining</returns>
    public static IHostApplicationBuilder UseEasyCachingRedis(
        this IHostApplicationBuilder builder,
        string providerName = "default",
        string? host = null,
        int? port = null,
        int? database = null,
        string? password = null)
    {
        builder.Services.AddEasyCaching(options =>
        {
            options.UseRedis(config =>
            {
                config.DBConfig.Endpoints.Add(new EasyCaching.Core.Configurations.ServerEndPoint(
                    host ?? builder.Configuration["Redis:Host"]!,
                    port ?? int.Parse(builder.Configuration["Redis:Port"]!)
                ));
                
                config.DBConfig.Database = database ?? int.Parse(builder.Configuration["Redis:Database"]! ?? "0");
                
                var pwd = password ?? builder.Configuration["Redis:Password"]!;
                if (!string.IsNullOrEmpty(pwd))
                {
                    config.DBConfig.Password = pwd;
                }
            }, providerName);
        });

        return builder;
    }

    /// <summary>
    /// Adds EasyCaching with hybrid (multi-level) cache.
    /// Combines local (InMemory) and distributed (Redis) cache for optimal performance.
    /// Local cache reduces network calls, distributed cache ensures consistency across instances.
    /// </summary>
    /// <param name="builder">Host application builder</param>
    /// <param name="hybridProviderName">Name of the hybrid provider (default: "hybrid")</param>
    /// <param name="localMaxSize">Maximum items in local cache (default: 1000)</param>
    /// <returns>Host application builder for chaining</returns>
    public static IHostApplicationBuilder UseEasyCachingHybrid(
        this IHostApplicationBuilder builder,
        string hybridProviderName = "hybrid",
        int localMaxSize = 1000)
    {
        builder.Services.AddEasyCaching(options =>
        {
            // Local cache (L1)
            options.UseInMemory(config =>
            {
                config.DBConfig = new InMemoryCachingOptions
                {
                    SizeLimit = localMaxSize,
                    ExpirationScanFrequency = 60
                };
            }, "local");

            // Distributed cache (L2)
            options.UseRedis(config =>
            {
                config.DBConfig.Endpoints.Add(new EasyCaching.Core.Configurations.ServerEndPoint(
                    builder.Configuration["Redis:Host"]!,
                    int.Parse(builder.Configuration["Redis:Port"]!)
                ));
                
                config.DBConfig.Database = int.Parse(builder.Configuration["Redis:Database"] ?? "0");
                
                if (!string.IsNullOrEmpty(builder.Configuration["Redis:Password"]))
                {
                    config.DBConfig.Password = builder.Configuration["Redis:Password"];
                }
            }, "distributed");

            // Hybrid cache combining both
            options.UseHybrid(config =>
            {
                config.LocalCacheProviderName = "local";
                config.DistributedCacheProviderName = "distributed";
                config.EnableLogging = true;
            }, hybridProviderName);
        });

        return builder;
    }
}
