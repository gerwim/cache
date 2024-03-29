﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerwimFeiken.Cache.Exceptions;
using GerwimFeiken.Cache.Models;
using GerwimFeiken.Cache.Redis.Options;
using GerwimFeiken.Cache.Utils;
using StackExchange.Redis;

namespace GerwimFeiken.Cache.Redis
{
     public class RedisCache : BaseCache
    {
        private readonly int _expirationTtl;
        private readonly bool _ignoreTimeouts;
        private readonly ConnectionMultiplexer _redis;
        
        public RedisCache(IRedisOptions options) : base(options)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));

            _redis = ConnectionMultiplexer.Connect(options.GetRequiredValue(x => x.Configuration)!);
            _expirationTtl = options.GetRequiredValue(x => x.DefaultExpirationTtl);
            _ignoreTimeouts = options.GetRequiredValue(x => x.IgnoreTimeouts);
        }
        
        protected override async Task DeleteImplementation(string key)
        {
            await DeleteImplementation([key]).ConfigureAwait(false);
        }

        protected override async Task DeleteImplementation(IEnumerable<string> keys)
        {
            try
            {
                var redisDb = _redis.GetDatabase();
                await redisDb.KeyDeleteAsync(keys.Select(x => new RedisKey(x)).ToArray()).ConfigureAwait(false);
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

        protected override Task<IEnumerable<string>> ListKeysImplementation(string? prefix)
        {
            var endpoints = _redis.GetEndPoints();
            var server = _redis.GetServer(endpoints[0]);

            var keys = server.Keys(pattern: $"{prefix}*").ToList();

            return Task.FromResult(keys.Select(x => x.ToString()));
        }

        protected override async Task<ReadResult> ReadImplementation(string key)
        {
            try
            {
                var redisDb = _redis.GetDatabase();
                var response = await redisDb.StringGetAsync(key).ConfigureAwait(false);

                if (!response.HasValue) return ReadResult.Fail(null, ReadReason.KeyDoesNotExist);
                
                return ReadResult.Ok(response.ToString());
            }
            catch (RedisConnectionException ex)
            {
                if (_ignoreTimeouts && ex.Message.StartsWith("The message timed out")) return ReadResult.Fail(null, ReadReason.Timeout);

                throw;
            }
            catch (RedisTimeoutException)
            {
                if (_ignoreTimeouts) return ReadResult.Fail(null, ReadReason.Timeout);

                throw;
            }
        }

        protected override async Task<WriteResult> WriteImplementation(string key, string value, int? expireInSeconds)
        {
            return await RedisWrite(key, value, expireInSeconds, When.Always).ConfigureAwait(false);
        }

        protected override async Task<WriteResult> WriteImplementation(string key, string value, bool errorIfExists, int? expireInSeconds)
        {
            return await RedisWrite(key, value, expireInSeconds, errorIfExists ? When.NotExists : When.Always).ConfigureAwait(false);
        }
        
        private async Task<WriteResult> RedisWrite(string key, string value, int? expireInSeconds, When when) {
            try
            {
                var redisDb = _redis.GetDatabase();

                var response =
                    await redisDb.StringSetAsync(key, value, TimeSpan.FromSeconds(expireInSeconds ?? _expirationTtl), when).ConfigureAwait(false);
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
    }
}