using System;
using System.Threading.Tasks;
using GerwimFeiken.Cache.Exceptions;
using GerwimFeiken.Cache.Models;
using GerwimFeiken.Cache.Redis.Options;
using GerwimFeiken.Cache.Utils;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace GerwimFeiken.Cache.Redis
{
     public class RedisCache : BaseCache
    {
        private readonly int _expirationTtl;
        private readonly bool _ignoreTimeouts;
        private readonly ConnectionMultiplexer _redis;
        
        public RedisCache(IRedisOptions options)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));

            _redis = ConnectionMultiplexer.Connect(options.GetRequiredValue(x => x.Configuration));
            _expirationTtl = options.GetRequiredValue(x => x.DefaultExpirationTtl);
            _ignoreTimeouts = options.GetRequiredValue(x => x.IgnoreTimeouts);
        }
        
        protected override async Task DeleteImplementation(string key)
        {
            try
            {
                var redisDb = _redis.GetDatabase();
                await redisDb.KeyDeleteAsync(key);
            }
            catch (RedisConnectionException ex)
            {
                if (_ignoreTimeouts && ex.Message.StartsWith("The message timed out")) return;

                throw;
            }
            catch (RedisTimeoutException)
            {
                if (_ignoreTimeouts) return;

                throw;
            }
        }

        protected override async Task<WriteResult> WriteImplementation<T>(string key, T value, int? expireInSeconds)
        {
            return await RedisWrite(key, value, expireInSeconds, When.Always);
        }

        protected override async Task<WriteResult> WriteImplementation<T>(string key, T value, bool errorIfExists, int? expireInSeconds)
        {
            return await RedisWrite(key, value, expireInSeconds, errorIfExists ? When.NotExists : When.Always);
        }
        
        private async Task<WriteResult> RedisWrite<T>(string key, T value, int? expireInSeconds, When when) {
            try
            {
                var redisDb = _redis.GetDatabase();
                string json = JsonConvert.SerializeObject(value, settings: new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                var response =
                    await redisDb.StringSetAsync(key, json, TimeSpan.FromSeconds(expireInSeconds ?? _expirationTtl), when);
                if (!response)
                {
                    if (when is When.NotExists) throw new KeyAlreadyExistsException();
                    throw new WriteException("Could not write to Redis");
                }
            }
            catch (RedisConnectionException ex)
            {
                if (_ignoreTimeouts && ex.Message.StartsWith("The message timed out")) return WriteResult.Fail(WriteReason.Timeout);

                throw;
            }
            catch (RedisTimeoutException)
            {
                if (_ignoreTimeouts) return WriteResult.Fail(WriteReason.Timeout);

                throw;
            }

            return WriteResult.Ok();
        }

        protected override async Task<ReadResult<T?>> ReadImplementation<T>(string key) where T : default
        {
            try
            {
                var redisDb = _redis.GetDatabase();
                var response = await redisDb.StringGetAsync(key);

                if (!response.HasValue) return ReadResult<T?>.Fail(default, ReadReason.KeyDoesNotExist);

                var obj = JsonConvert.DeserializeObject<T>(response.ToString());
                return ReadResult<T?>.Ok(obj);
            }
            catch (RedisConnectionException ex)
            {
                if (_ignoreTimeouts && ex.Message.StartsWith("The message timed out")) return ReadResult<T?>.Fail(default, ReadReason.Timeout);

                throw;
            }
            catch (RedisTimeoutException)
            {
                if (_ignoreTimeouts) return ReadResult<T?>.Fail(default, ReadReason.Timeout);

                throw;
            }
        }
    }
}