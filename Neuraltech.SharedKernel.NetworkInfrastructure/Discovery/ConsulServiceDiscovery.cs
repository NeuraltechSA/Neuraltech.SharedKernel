using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Neuraltech.SharedKernel.NetworkInfrastructure.Yarp;

namespace Neuraltech.SharedKernel.NetworkInfrastructure.Discovery;

public class ConsulServiceDiscovery(
    ConfigProvider configProvider,
    IConfiguration configuration,
    ILogger<ConsulServiceDiscovery> logger
) : BackgroundService
{
    private readonly TimeSpan _refreshInterval = TimeSpan.FromSeconds(
        configuration.GetValue("ServiceDiscovery:RefreshIntervalSeconds", 30)
    );

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "Consul Service Discovery started. Refreshing every {Interval}s",
            _refreshInterval.TotalSeconds
        );

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_refreshInterval, stoppingToken);
                await configProvider.DiscoverServices(stoppingToken);
                logger.LogInformation("YARP destinations refreshed from Consul");
            }
            catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Shutdown requested, exit gracefully
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error refreshing destinations from Consul. Retrying in {Interval}s",
                    _refreshInterval.TotalSeconds
                );
            }
        }

        logger.LogInformation("Consul Service Discovery stopped");
    }
}
