using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GerwimFeiken.Cache.Implementations
{
    public class InMemoryCache : BaseCache
    {
        private readonly int _expirationTtl;
        private ConcurrentDictionary<string, (DateTime expireAtUtc, string data)> LocalCache { get; }
        
        public InMemoryCache(IConfiguration configuration)
        {
            LocalCache = new ConcurrentDictionary<string, (DateTime expireAtUtc, string data)>();
            try
            {
                _expirationTtl = string.IsNullOrWhiteSpace(configuration["GerwimFeiken.Cache:DefaultExpirationTtl"])
                    ? 86400
                    : Convert.ToInt32(configuration["GerwimFeiken.Cache:DefaultExpirationTtl"]);
            }
            catch
            {
                _expirationTtl = 86400;
            }
        }
        protected override Task WriteImplementation<T>(string key, T value, int? expireInSeconds)
        {
            LocalCache[key] = (DateTime.UtcNow.AddSeconds(expireInSeconds ?? _expirationTtl), JsonConvert.SerializeObject(value, settings: new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
            return Task.CompletedTask;
        }

        protected override Task<T> ReadImplementation<T>(string key)
        {
            if (!LocalCache.TryGetValue(key, out var value))
                return Task.FromResult<T>(default);

            if (DateTime.UtcNow <= value.expireAtUtc)
                return Task.FromResult(JsonConvert.DeserializeObject<T>(value.data));
            
            LocalCache.TryRemove(key, out _);
            return Task.FromResult<T>(default);
        }
    }
}