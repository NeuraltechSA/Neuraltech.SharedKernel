using Neuraltech.SharedKernel.NetworkInfrastructure.Discovery;
using Neuraltech.SharedKernel.NetworkInfrastructure.Yarp;
using Steeltoe.Discovery.Consul;
using Yarp.ReverseProxy.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class NetworkInfrastructureExtensions
{
    public static IServiceCollection UseConsulServiceDiscovery(this IServiceCollection services)
    {
        services.AddConsulDiscoveryClient();
        services.AddSingleton<ConfigProvider>();
        services.AddSingleton<IProxyConfigProvider>(sp => sp.GetRequiredService<ConfigProvider>());
        services.AddHostedService<ConsulServiceDiscovery>();
        return services;
    }

    public static IServiceCollection UseYarpProxy(this IServiceCollection services)
    {
        services.AddReverseProxy();
        services.AddControllers();
        return services;
    }
}
