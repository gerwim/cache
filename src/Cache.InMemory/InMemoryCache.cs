using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using GerwimFeiken.Cache.InMemory.Options;
using Newtonsoft.Json;

namespace GerwimFeiken.Cache.InMemory
{
    public class InMemoryCache : BaseCache
    {
        private readonly IInMemoryOptions _options;
        private static ConcurrentDictionary<string, (DateTime expireAtUtc, string data)> LocalCache { get; } = new();
        
        public InMemoryCache(IInMemoryOptions options)
        {
            _options = options;
        }

        protected override Task DeleteImplementation(string key)
        {
            LocalCache.TryRemove(key, out _);
            return Task.CompletedTask;
        }

        protected override Task WriteImplementation<T>(string key, T value, int? expireInSeconds)
        {
            LocalCache[key] = (DateTime.UtcNow.AddSeconds(expireInSeconds ?? _options.DefaultExpirationTtl), JsonConvert.SerializeObject(value, settings: new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
            return Task.CompletedTask;
        }

        protected override Task<T?> ReadImplementation<T>(string key) where T : default
        {
            if (!LocalCache.TryGetValue(key, out var value))
                return Task.FromResult<T?>(default);

            if (DateTime.UtcNow <= value.expireAtUtc)
                return Task.FromResult(JsonConvert.DeserializeObject<T?>(value.data));
            
            LocalCache.TryRemove(key, out _);
            return Task.FromResult<T?>(default);
        }
    }
}