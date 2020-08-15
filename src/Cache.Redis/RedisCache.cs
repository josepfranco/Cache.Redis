using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Abstractions.Cache;
using Cache.Redis.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Cache.Redis
{
    public class RedisCache : ICache
    {
        private readonly IDatabaseAsync _cache;

        public RedisCache(IOptions<RedisConfiguration> configurationOption)
        {
            if (configurationOption == null) 
                throw new ArgumentNullException(nameof(RedisConfiguration), $"{nameof(RedisConfiguration)} options object not correctly setup.");

            var redisConfiguration = configurationOption.Value;
            var connectionString = string.IsNullOrEmpty(redisConfiguration.ConnectionString) 
                ? throw new ArgumentException($"{nameof(RedisConfiguration)} connection string cannot be null or empty.", nameof(RedisConfiguration))
                : redisConfiguration.ConnectionString;
            
            var multiplexer = ConnectionMultiplexer.Connect(ConfigurationOptions.Parse(connectionString));
            _cache = multiplexer.GetDatabase();
        }


        public async Task<TObject> GetAsync<TObject>(string key, CancellationToken token = default)
            where TObject : class
        {
            var redisValue = await _cache.StringGetAsync(new RedisKey(key));
            
            // has no value? return null as per the contract description
            if (redisValue.IsNullOrEmpty) return null;
            
            // deserialize the inserted object
            var rawValue = redisValue.ToString();
            return JsonSerializer.Deserialize<TObject>(rawValue);
        }

        public Task<bool> SetAsync(string key, object value, TimeSpan? expiry = null, CancellationToken token = default)
        {
            var serializedValue = JsonSerializer.Serialize(value);
            return _cache.StringSetAsync(
                new RedisKey(key), 
                new RedisValue(serializedValue), 
                expiry);
        }
    }
}