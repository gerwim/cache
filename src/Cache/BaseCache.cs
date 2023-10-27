using System;
using System.Threading.Tasks;
using GerwimFeiken.Cache.Models;
using GerwimFeiken.Cache.Utils.Extensions;

namespace GerwimFeiken.Cache
{
    public abstract class BaseCache : ICache
    {
        public Task Write<T>(string key, T value, int? expireInSeconds = null)
        {
            // Convert the key
            string encodedKey = $"{typeof(T)}-{key}".ComputeSha256Hash();
            
            return WriteImplementation(encodedKey, value, expireInSeconds);
        }
        
        public Task Write<T>(string key, T value, TimeSpan expireIn)
        {
            var seconds = expireIn.TotalSeconds;
            if (seconds > Int32.MaxValue) seconds = Int32.MaxValue;
            
            return Write(key, value, (int) seconds);
        }

        public Task Write<T>(string key, T value, bool errorIfExists, int? expireInSeconds = null)
        {
            // Convert the key
            string encodedKey = $"{typeof(T)}-{key}".ComputeSha256Hash();
            
            return WriteImplementation(encodedKey, value, errorIfExists, expireInSeconds);
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

        public async Task<T?> Read<T>(string key)
        {
            // Convert the key
            string encodedKey = $"{typeof(T)}-{key}".ComputeSha256Hash();

            return (await ReadImplementation<T?>(encodedKey).ConfigureAwait(false)).Value;
        }
        
        public Task Delete<T>(string key)
        {
            // Convert the key
            string encodedKey = $"{typeof(T)}-{key}".ComputeSha256Hash();
            
            return DeleteImplementation(encodedKey);
        }

        protected abstract Task DeleteImplementation(string key);
        protected abstract Task<ReadResult<T?>> ReadImplementation<T>(string key);
        protected abstract Task<WriteResult> WriteImplementation<T>(string key, T value, int? expireInSeconds);
        protected abstract Task<WriteResult> WriteImplementation<T>(string key, T value, bool errorIfExists, int? expireInSeconds);
    }
}