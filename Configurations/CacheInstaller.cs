using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RedisCache.Services;
using StackExchange.Redis;

namespace RedisCache.Configurations
{
    public class CacheInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var redisConfiguration = new RedisConfiguration(); 

            configuration.GetSection("RedisConfiguration").Bind(redisConfiguration);

            services.AddSingleton(redisConfiguration);

            if(!redisConfiguration.Enable)
            {
                return;
            }

            services.AddSingleton<IConnectionMultiplexer>( _ => ConnectionMultiplexer.Connect(redisConfiguration.ConnectionString));
            services.AddStackExchangeRedisCache(option => option.Configuration = redisConfiguration.ConnectionString);
            services.AddSingleton<IResponseCacheService , ResponseCacheService>();
        }
    }
}