using Abstractions.Cache;
using Cache.Redis.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Cache.Redis.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers Redis' distributed cache implementation into the DI
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddDistributedRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RedisConfiguration>(configuration.GetSection(nameof(RedisConfiguration)));
            services.TryAddSingleton<ICache, RedisCache>();
            return services;
        }
    }
}