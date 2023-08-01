using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using GerwimFeiken.Cache.Exceptions;
using GerwimFeiken.Cache.InMemory.Options;
using Newtonsoft.Json;

namespace GerwimFeiken.Cache.InMemory
{
    public class InMemoryCache : BaseCache
    {
        private readonly IInMemoryOptions _options;
        private static ConcurrentDictionary<string, (DateTime expireAtUtc, string data)> LocalCache { get; } = new();
        private static readonly SemaphoreSlim WriteLock = new(1, 1);
        
        public InMemoryCache(IInMemoryOptions options)
        {
            _options = options;
        }

        protected override Task DeleteImplementation(string key)
        {
            LocalCache.TryRemove(key, out _);
            return Task.CompletedTask;
        }

        protected override async Task WriteImplementation<T>(string key, T value, int? expireInSeconds)
        {
            try
            {
                await WriteLock.WaitAsync();
                WriteToLocalDictionary(key, value, expireInSeconds);
            }
            finally
            {
                WriteLock.Release();
            }
        }

        protected override async Task WriteImplementation<T>(string key, T value, bool errorIfExists, int? expireInSeconds)
        {
            if (!errorIfExists)
            {
                await WriteImplementation(key, value, expireInSeconds);
                return;
            }
            
            try
            {
                await WriteLock.WaitAsync();
                
                // Read key -- to make sure it's deleted if expired and to check whether it exists
                var existingValue = await ReadImplementation<T>(key);
                if (!Equals(existingValue, default(T)))
                {
                    throw new KeyAlreadyExistsException();
                }
                
                WriteToLocalDictionary(key, value, expireInSeconds);
            }
            finally
            {
                WriteLock.Release();
            }
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

        /// <summary>
        /// Write to local dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireInSeconds"></param>
        /// <typeparam name="T"></typeparam>
        private void WriteToLocalDictionary<T>(string key, T value, int? expireInSeconds)
        {
            LocalCache[key] = (DateTime.UtcNow.AddSeconds(expireInSeconds ?? _options.DefaultExpirationTtl),
                JsonConvert.SerializeObject(value, settings: new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }));
        }
    }
}