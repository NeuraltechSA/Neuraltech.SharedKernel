using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion.Locking.Distributed.Redis;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace Neuraltech.SharedKernel.Infraestructure.Extensions
{
    public static class CacheExtensions
    {
        public static IFusionCacheBuilder UseFusionCache(
            this IHostApplicationBuilder builder,
            string cacheName/* = "Default"*/)
        {
            var redisConnStr = builder.Configuration.GetConnectionString("Redis");

            return builder.Services.AddFusionCache(cacheName)
                .WithCacheKeyPrefixByCacheName()
                .WithSerializer(new FusionCacheSystemTextJsonSerializer())
                .WithDistributedCache(new RedisCache(
                    new RedisCacheOptions()
                    {
                        Configuration = redisConnStr
                    })
                )
                .WithBackplane(new RedisBackplane(
                    new RedisBackplaneOptions
                    {
                        Configuration = redisConnStr
                    }    
                ))
                .WithDistributedLocker(new RedisDistributedLocker(
                    new RedisDistributedLockerOptions
                    {
                        Configuration = redisConnStr
                    }    
                ));
        }
        /*
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
        }*/
    }
}
