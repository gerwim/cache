using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GerwimFeiken.Cache.ContractResolvers;
using GerwimFeiken.Cache.Exceptions;
using GerwimFeiken.Cache.Models;
using Newtonsoft.Json;

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

        public Task<IEnumerable<string>> ListKeys(string? prefix = null)
        {
            return ListKeysImplementation(prefix);
        }

        public async Task<T?> Read<T>(string key)
        {
            try
            {
                return (await ReadImplementation<T?>(key).ConfigureAwait(false)).Value;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Could not convert"))
                {
                    throw new InvalidTypeException($"The value of key being queried is not of type {typeof(T)}");
                }

                throw;
            }
        }

        public Task<dynamic?> Read(string key)
        {
            return Read<dynamic?>(key);
        }
        
		[Obsolete("Please use the non generic method")]
        public Task Delete<T>(string key)
        {
            return Delete(key);
        }
        
        public Task Delete(string key)
        {
            return DeleteImplementation(key);
        }
        
        public Task Delete(IEnumerable<string> keys)
        {
            return DeleteImplementation(keys);
        }

        protected abstract Task DeleteImplementation(string key);
        protected abstract Task DeleteImplementation(IEnumerable<string> keys);
        protected abstract Task<IEnumerable<string>> ListKeysImplementation(string? prefix);
        protected abstract Task<ReadResult<T?>> ReadImplementation<T>(string key);
        protected abstract Task<WriteResult> WriteImplementation<T>(string key, T value, int? expireInSeconds);
        protected abstract Task<WriteResult> WriteImplementation<T>(string key, T value, bool errorIfExists, int? expireInSeconds);

        /// <summary>
        /// Serializes the object using Newtonsoft.Json
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual string? SerializeObject(object? value)
        {
            if (value is null) return null;
            
            return JsonConvert.SerializeObject(value, settings: new JsonSerializerSettings
            {
                ContractResolver = new PrivateSetterAndCtorContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            });
        }
        
        /// <summary>
        /// Deserializes the object using Newtonsoft.Json
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual T? DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, new JsonSerializerSettings
            {
                ContractResolver = new PrivateSetterAndCtorContractResolver(),
            });
        }
    }
}