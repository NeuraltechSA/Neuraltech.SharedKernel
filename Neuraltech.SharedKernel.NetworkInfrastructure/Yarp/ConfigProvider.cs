using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Steeltoe.Common.Discovery;
using Yarp.ReverseProxy.Configuration;

namespace Neuraltech.SharedKernel.NetworkInfrastructure.Yarp;

public class ConfigProvider : IProxyConfigProvider
{
    private readonly IConfiguration _configuration;
    private Config? _config;
    private readonly IDiscoveryClient _discoveryClient;

    public ConfigProvider(IConfiguration configuration, IDiscoveryClient discoveryClient)
    {
        _configuration = configuration;
        _discoveryClient = discoveryClient;
        DiscoverServices().Wait();
    }

    public async Task DiscoverServices(CancellationToken? cancellationToken = null)
    {
        // Store reference to current config (the one YARP is holding)
        var currentConfig = _config;

        // Load fresh base config from appsettings
        var baseConfig = Config.LoadConfig(_configuration);

        var updatedClusters = new List<ClusterConfig>();

        // Merge config
        foreach (var cluster in baseConfig.Clusters)
        {
            // ClusterId = Consul service name (e.g. "blog-service")
            var instances = await _discoveryClient.GetInstancesAsync(
                cluster.ClusterId,
                cancellationToken ?? CancellationToken.None
            );

            if (instances != null && instances.Count > 0)
            {
                // Build new Destinations from discovered instances
                var destinations = new Dictionary<string, DestinationConfig>();
                for (var i = 0; i < instances.Count; i++)
                {
                    var instance = instances[i];
                    var id = instance.InstanceId ?? $"{cluster.ClusterId}-{i}";
                    destinations[id] = new DestinationConfig { Address = instance.Uri.ToString() };
                }

                // Replace cluster destinations with discovered ones
                updatedClusters.Add(cluster with { Destinations = destinations });
            }
            else
            {
                // No instances found, keep original config as fallback
                updatedClusters.Add(cluster);
            }
        }

        // Swap config with updated clusters
        // Update member variable so next GetConfig() returns this new one
        _config = new Config(baseConfig.Routes.ToList(), updatedClusters);

        // Signal the OLD config that a change has occurred
        currentConfig?.SignalChange();
    }

    public IProxyConfig GetConfig()
    {
        return _config!;
    }
}
