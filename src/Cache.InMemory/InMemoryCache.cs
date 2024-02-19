using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GerwimFeiken.Cache.Exceptions;
using GerwimFeiken.Cache.InMemory.Options;
using GerwimFeiken.Cache.Models;

namespace GerwimFeiken.Cache.InMemory
{
    public class InMemoryCache : BaseCache
    {
        private readonly IInMemoryOptions _options;
        private static ConcurrentDictionary<string, (DateTime expireAtUtc, string data)> LocalCache { get; } = new();
        private static readonly SemaphoreSlim WriteLock = new(1, 1);
        
        public InMemoryCache(IInMemoryOptions options) : base(options)
        {
            _options = options;
        }

        protected override Task DeleteImplementation(string key)
        {
            LocalCache.TryRemove(key, out _);
            return Task.CompletedTask;
        }

        protected override Task DeleteImplementation(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                DeleteImplementation(key);
            }

            return Task.CompletedTask;
        }

        protected override async Task<IEnumerable<string>> ListKeysImplementation(string? prefix)
        {
            var keys = new List<string>();
            foreach (var s in LocalCache.Keys.Where(x => string.IsNullOrWhiteSpace(prefix) || x.StartsWith(prefix)))
            {
                var result = await ReadImplementation(s).ConfigureAwait(false);
                if (result.OperationStatus is Status.Ok) keys.Add(s);
            }
            
            return keys;
        }

        protected override Task<ReadResult> ReadImplementation(string key)
        {
            if (!LocalCache.TryGetValue(key, out var value))
                return Task.FromResult(ReadResult.Fail(null, ReadReason.KeyDoesNotExist));

            if (DateTime.UtcNow <= value.expireAtUtc)
                return Task.FromResult(ReadResult.Ok(value.data));
            
            LocalCache.TryRemove(key, out _);
            return Task.FromResult(ReadResult.Fail(null, ReadReason.KeyDoesNotExist));
        }

        protected override async Task<WriteResult> WriteImplementation(string key, string value, int? expireInSeconds)
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

        protected override async Task<WriteResult> WriteImplementation(string key, string value, bool errorIfExists, int? expireInSeconds)
        {
            if (!errorIfExists)
            {
                return await WriteImplementation(key, value, expireInSeconds).ConfigureAwait(false);
            }

            try
            {
                await WriteLock.WaitAsync().ConfigureAwait(false);

                // Read key -- to make sure it's deleted if expired
                _ = await ReadImplementation(key).ConfigureAwait(false);

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
        
        private (DateTime expireAtUtc, string? data) ConvertValue(string value, int? expireInSeconds)
        {
            return (DateTime.UtcNow.AddSeconds(expireInSeconds ?? _options.DefaultExpirationTtl),
                    value
                );
        }
    }
}