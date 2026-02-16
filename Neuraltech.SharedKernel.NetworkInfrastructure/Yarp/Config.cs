using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

namespace Neuraltech.SharedKernel.NetworkInfrastructure.Yarp;

public class Config : IProxyConfig
{
    private readonly CancellationTokenSource _cts = new();

    public IReadOnlyList<RouteConfig> Routes { get; }
    public IReadOnlyList<ClusterConfig> Clusters { get; }

    public IChangeToken ChangeToken => new CancellationChangeToken(_cts.Token);

    public Config(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
    {
        Routes = routes;
        Clusters = clusters;
    }

    public static Config LoadConfig(IConfiguration configuration)
    {
        var routes = new List<RouteConfig>();
        var clusters = new List<ClusterConfig>();
        var section = configuration.GetSection("ReverseProxy");

        // Parse Routes
        foreach (var child in section.GetSection("Routes").GetChildren())
        {
            var route = child.Get<RouteConfig>();
            if (route != null)
            {
                if (string.IsNullOrEmpty(route.RouteId))
                    route = route with { RouteId = child.Key };
                routes.Add(route);
            }
        }

        // Parse Clusters
        foreach (var child in section.GetSection("Clusters").GetChildren())
        {
            var cluster = child.Get<ClusterConfig>();
            if (cluster != null)
            {
                if (string.IsNullOrEmpty(cluster.ClusterId))
                    cluster = cluster with { ClusterId = child.Key };
                clusters.Add(cluster);
            }
        }

        return new Config(routes, clusters);
    }

    internal void SignalChange() => _cts.Cancel();
}
