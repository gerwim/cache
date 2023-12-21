using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GerwimFeiken.Cache.Models;

namespace GerwimFeiken.Cache
{
    public abstract class BaseCache : ICache
    {
        public Task Write<T>(string key, T value, int? expireInSeconds = null)
        {
            return WriteImplementation(key, value, expireInSeconds);
        }
        
        public Task Write<T>(string key, T value, TimeSpan expireIn)
        {
            var seconds = expireIn.TotalSeconds;
            if (seconds > Int32.MaxValue) seconds = Int32.MaxValue;
            
            return Write(key, value, (int) seconds);
        }

        public Task Write<T>(string key, T value, bool errorIfExists, int? expireInSeconds = null)
        {
            return WriteImplementation(key, value, errorIfExists, expireInSeconds);
        }
        
        public Task Write<T>(string key, T value, bool errorIfExists, TimeSpan expireIn)
        {
            var seconds = expireIn.TotalSeconds;
            if (seconds > Int32.MaxValue) seconds = Int32.MaxValue;
            
            return Write(key, value, errorIfExists, (int) seconds);
        }

        public async Task<T?> ReadOrWrite<T>(string key, Func<T> func, int? expireInSeconds = null)
        {
            var existingValue = await Read<T>(key).ConfigureAwait(false);
            if (existingValue is not null) return existingValue;
            
            var result = func();
            
            if (result is not null) await Write(key, result, expireInSeconds).ConfigureAwait(false);
            return result;
        }

        public async Task<T?> ReadOrWrite<T>(string key, Func<Task<T>> func, int? expireInSeconds = null)
        {
            var existingValue = await Read<T>(key).ConfigureAwait(false);
            if (existingValue is not null) return existingValue;

            var result = await func().ConfigureAwait(false);

            if (result is not null) await Write(key, result, expireInSeconds).ConfigureAwait(false);
            return result;
        }
        
        public async Task<T?> ReadOrWrite<T>(string key, Func<T> func, TimeSpan expireIn)
        {
            var seconds = expireIn.TotalSeconds;
            if (seconds > Int32.MaxValue) seconds = Int32.MaxValue;
            
            return await ReadOrWrite(key, func, (int) seconds).ConfigureAwait(false);
        }
        
        public async Task<T?> ReadOrWrite<T>(string key, Func<Task<T>> func, TimeSpan expireIn)
        {
            var seconds = expireIn.TotalSeconds;
            if (seconds > Int32.MaxValue) seconds = Int32.MaxValue;
            
            return await ReadOrWrite(key, func, (int) seconds).ConfigureAwait(false);
        }

        public Task<IEnumerable<string>> ListKeys(string prefix)
        {
            return ListKeysImplementation(prefix);
        }

        public async Task<T?> Read<T>(string key)
        {
            return (await ReadImplementation<T?>(key).ConfigureAwait(false)).Value;
        }
        
        [Obsolete]
        public Task Delete<T>(string key)
        {
            return DeleteImplementation(key);
        }

        protected abstract Task DeleteImplementation(string key);
        protected abstract Task<IEnumerable<string>> ListKeysImplementation(string prefix);
        protected abstract Task<ReadResult<T?>> ReadImplementation<T>(string key);
        protected abstract Task<WriteResult> WriteImplementation<T>(string key, T value, int? expireInSeconds);
        protected abstract Task<WriteResult> WriteImplementation<T>(string key, T value, bool errorIfExists, int? expireInSeconds);
    }
}