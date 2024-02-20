using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GerwimFeiken.Cache.Exceptions;
using GerwimFeiken.Cache.Models;
using GerwimFeiken.Cache.Options;
using GerwimFeiken.Cache.SerializerSettings;
using Newtonsoft.Json;

namespace GerwimFeiken.Cache;

public abstract class BaseCache : ICache
{
    private readonly JsonSerializerSettings _jsonSerializerSettings;

    protected BaseCache(IOptions options)
    {
        _jsonSerializerSettings = options.JsonSerializerSettings ??
                                  new DefaultSerializerSettings();
    }

    public Task Write<T>(string key, T value, int? expireInSeconds = null)
    {
        var json = SerializeObject(value);
        if (json is null) return Task.CompletedTask;

        return WriteImplementation(key, json, expireInSeconds);
    }

    public Task Write<T>(string key, T value, TimeSpan expireIn)
    {
        var seconds = expireIn.TotalSeconds;
        if (seconds > int.MaxValue) seconds = int.MaxValue;

        return Write(key, value, (int)seconds);
    }

    public Task Write<T>(string key, T value, bool errorIfExists, int? expireInSeconds = null)
    {
        var json = SerializeObject(value);
        if (json is null) return Task.CompletedTask;

        return WriteImplementation(key, json, errorIfExists, expireInSeconds);
    }

    public Task Write<T>(string key, T value, bool errorIfExists, TimeSpan expireIn)
    {
        var seconds = expireIn.TotalSeconds;
        if (seconds > int.MaxValue) seconds = int.MaxValue;

        return Write(key, value, errorIfExists, (int)seconds);
    }

    public Task<dynamic?> Read(string key)
    {
        return Read<dynamic>(key);
    }

    public async Task<T?> Read<T>(string key, T? ignored = null) where T : struct
    {
        try
        {
            var json = (await ReadImplementation(key).ConfigureAwait(false)).Value;
            if (json is null) return null;

            return DeserializeObject<T>(json);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Could not convert"))
                throw new InvalidTypeException($"The value of key being queried is not of type {typeof(T)}");

            throw;
        }
    }


    public async Task<T?> Read<T>(string key, ClassConstraint<T>? ignored = null) where T : class?
    {
        try
        {
            var json = (await ReadImplementation(key).ConfigureAwait(false)).Value;
            if (json is null) return null;

            return DeserializeObject<T>(json);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Could not convert"))
                throw new InvalidTypeException($"The value of key being queried is not of type {typeof(T)}");

            throw;
        }
    }

    public Task<IEnumerable<string>> ListKeys(string? prefix = null)
    {
        return ListKeysImplementation(prefix);
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

    public async Task<T?> ReadOrWrite<T>(string key, Func<T?> func, int? expireInSeconds = null, T? ignored = null)
        where T : struct
    {
        var existingValue = await Read<T>(key).ConfigureAwait(false);
        if (existingValue is not null) return existingValue;

        var result = func();

        if (result is not null) await Write(key, result, expireInSeconds).ConfigureAwait(false);
        return result;
    }

    public async Task<T?> ReadOrWrite<T>(string key, Func<T?> func, int? expireInSeconds = null,
        ClassConstraint<T>? ignored = null) where T : class?
    {
        var existingValue = await Read<T>(key).ConfigureAwait(false);
        if (existingValue is not null) return existingValue;

        var result = func();

        if (result is not null) await Write(key, result, expireInSeconds).ConfigureAwait(false);
        return result;
    }

    public async Task<T?> ReadOrWrite<T>(string key, Func<Task<T?>> func, int? expireInSeconds = null,
        T? ignored = null) where T : struct
    {
        var existingValue = await Read<T>(key).ConfigureAwait(false);
        if (existingValue is not null) return existingValue;

        var result = await func().ConfigureAwait(false);

        if (result is not null) await Write(key, result, expireInSeconds).ConfigureAwait(false);
        return result;
    }

    public async Task<T?> ReadOrWrite<T>(string key, Func<Task<T?>> func, int? expireInSeconds = null,
        ClassConstraint<T>? ignored = null) where T : class?
    {
        var existingValue = await Read<T>(key).ConfigureAwait(false);
        if (existingValue is not null) return existingValue;

        var result = await func().ConfigureAwait(false);

        if (result is not null) await Write(key, result, expireInSeconds).ConfigureAwait(false);
        return result;
    }

    public async Task<T?> ReadOrWrite<T>(string key, Func<T?> func, TimeSpan expireIn, T? ignored = null)
        where T : struct
    {
        var seconds = expireIn.TotalSeconds;
        if (seconds > int.MaxValue) seconds = int.MaxValue;

        return await ReadOrWrite(key, func, (int)seconds).ConfigureAwait(false);
    }

    public async Task<T?> ReadOrWrite<T>(string key, Func<T?> func, TimeSpan expireIn,
        ClassConstraint<T>? ignored = null) where T : class?
    {
        var seconds = expireIn.TotalSeconds;
        if (seconds > int.MaxValue) seconds = int.MaxValue;

        return await ReadOrWrite(key, func, (int)seconds).ConfigureAwait(false);
    }

    public async Task<T?> ReadOrWrite<T>(string key, Func<Task<T?>> func, TimeSpan expireIn, T? ignored = null)
        where T : struct
    {
        var seconds = expireIn.TotalSeconds;
        if (seconds > int.MaxValue) seconds = int.MaxValue;

        return await ReadOrWrite(key, func, (int)seconds).ConfigureAwait(false);
    }

    public async Task<T?> ReadOrWrite<T>(string key, Func<Task<T?>> func, TimeSpan expireIn,
        ClassConstraint<T>? ignored = null) where T : class?
    {
        var seconds = expireIn.TotalSeconds;
        if (seconds > int.MaxValue) seconds = int.MaxValue;

        return await ReadOrWrite(key, func, (int)seconds).ConfigureAwait(false);
    }

    protected abstract Task DeleteImplementation(string key);
    protected abstract Task DeleteImplementation(IEnumerable<string> keys);
    protected abstract Task<IEnumerable<string>> ListKeysImplementation(string? prefix);
    protected abstract Task<ReadResult> ReadImplementation(string key);
    protected abstract Task<WriteResult> WriteImplementation(string key, string value, int? expireInSeconds);

    protected abstract Task<WriteResult> WriteImplementation(string key, string value, bool errorIfExists,
        int? expireInSeconds);

    /// <summary>
    ///     Serializes the object using Newtonsoft.Json
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected virtual string? SerializeObject(object? value)
    {
        if (value is null) return null;

        return JsonConvert.SerializeObject(value, _jsonSerializerSettings);
    }

    /// <summary>
    ///     Deserializes the object using Newtonsoft.Json
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected virtual T? DeserializeObject<T>(string value)
    {
        return JsonConvert.DeserializeObject<T>(value, _jsonSerializerSettings);
    }
}