using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZiggyCreatures.Caching.Fusion;

namespace Neuraltech.SharedKernel.Infraestructure.Extensions
{
    public static class CacheExtensions
    {
        public static IFusionCacheBuilder UseFusionCache(
            this IHostApplicationBuilder builder)
        {
            return builder.Services.AddFusionCache()
                //.WithBackplane()
                .WithOptions(o =>
                {
                   
                })
                .WithMemoryCache(new MemoryCache(new MemoryCacheOptions
                {
                    
                }))
                .AsHybridCache();
        }
    }
}
