using System;
using System.Threading.Tasks;
using GerwimFeiken.Cache.Exceptions;
using GerwimFeiken.Cache.Redis.Options;
using GerwimFeiken.Cache.Utils;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace GerwimFeiken.Cache.Redis
{
     public class RedisCache : BaseCache
    {
        private readonly int _expirationTtl;
        private readonly ConnectionMultiplexer _redis;
        
        public RedisCache(IRedisOptions options)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));

            _redis = ConnectionMultiplexer.Connect(options.GetRequiredValue(x => x.Configuration));
            _expirationTtl = options.GetRequiredValue(x => x.DefaultExpirationTtl);
        }
        
        protected override async Task DeleteImplementation(string key)
        {
            var redisDb = _redis.GetDatabase();
            await redisDb.KeyDeleteAsync(key);
        }

        protected override async Task WriteImplementation<T>(string key, T value, int? expireInSeconds)
        {
            var redisDb = _redis.GetDatabase();
            string json = JsonConvert.SerializeObject(value, settings: new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            
            var response = await redisDb.StringSetAsync(key, json, TimeSpan.FromSeconds(expireInSeconds ?? _expirationTtl));
            if (!response)
            {
                throw new WriteException("Could not write to Redis");
            }
        }
        
        protected override async Task<T?> ReadImplementation<T>(string key) where T : default
        {
            var redisDb = _redis.GetDatabase();
            var response = await redisDb.StringGetAsync(key);
            
            if (!response.HasValue) return default;

            var obj = JsonConvert.DeserializeObject<T>(response.ToString());
            return obj;
        }
    }
}