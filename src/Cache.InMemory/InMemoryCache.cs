using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GerwimFeiken.Cache.Exceptions;
using GerwimFeiken.Cache.InMemory.Options;
using GerwimFeiken.Cache.Models;
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

        protected override Task<IEnumerable<string>> ListKeysImplementation(string prefix)
        {
            throw new NotImplementedException();
        }

        protected override async Task<WriteResult> WriteImplementation<T>(string key, T value, int? expireInSeconds)
        {
            try
            {
                await WriteLock.WaitAsync().ConfigureAwait(false);
                LocalCache[key] = ConvertValue(value, expireInSeconds);
            }
            catch
            {
                return WriteResult.Fail();
            }
            finally
            {
                WriteLock.Release();
            }
            
            return WriteResult.Ok();
        }

        protected override async Task<WriteResult> WriteImplementation<T>(string key, T value, bool errorIfExists, int? expireInSeconds)
        {
            if (!errorIfExists)
            {
                return await WriteImplementation(key, value, expireInSeconds).ConfigureAwait(false);
            }

            try
            {
                await WriteLock.WaitAsync().ConfigureAwait(false);

                // Read key -- to make sure it's deleted if expired
                _ = await ReadImplementation<T>(key).ConfigureAwait(false);

                if (!LocalCache.TryAdd(key, ConvertValue(value, expireInSeconds)))
                {
                    throw new KeyAlreadyExistsException();
                }
            }
            finally
            {
                WriteLock.Release();
            }

            return WriteResult.Ok();
        }

        protected override Task<ReadResult<T?>> ReadImplementation<T>(string key) where T : default
        {
            if (!LocalCache.TryGetValue(key, out var value))
                return Task.FromResult(ReadResult<T?>.Fail(default, ReadReason.KeyDoesNotExist));

            if (DateTime.UtcNow <= value.expireAtUtc)
                return Task.FromResult(ReadResult<T?>.Ok(JsonConvert.DeserializeObject<T?>(value.data)));
            
            LocalCache.TryRemove(key, out _);
            return Task.FromResult(ReadResult<T?>.Fail(default, ReadReason.KeyDoesNotExist));
        }

        private (DateTime expireAtUtc, string data) ConvertValue<T>(T value, int? expireInSeconds)
        {
            return (DateTime.UtcNow.AddSeconds(expireInSeconds ?? _options.DefaultExpirationTtl),
                JsonConvert.SerializeObject(value, settings: new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }));
        }
    }
}