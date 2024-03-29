﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GerwimFeiken.Cache.Exceptions;
using GerwimFeiken.Cache.Models;

namespace GerwimFeiken.Cache;

public interface ICache
{
    /// <summary>
    ///     Write an object into cache
    /// </summary>
    /// <param name="key">Key of the object.</param>
    /// <param name="value">Object to cache.</param>
    /// <param name="expireInSeconds">Expiration of the object in seconds.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task Write<T>(string key, T value, int? expireInSeconds = null);

    /// <summary>
    ///     Write an object into cache
    /// </summary>
    /// <param name="key">Key of the object.</param>
    /// <param name="value">Object to cache.</param>
    /// <param name="expireIn">Expiration of the object.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task Write<T>(string key, T value, TimeSpan expireIn);

    /// <summary>
    ///     Write an object into cache
    /// </summary>
    /// <param name="key">Key of the object.</param>
    /// <param name="value">Object to cache.</param>
    /// <param name="errorIfExists">Throw a KeyAlreadyExistsException when the key already exists in the cache.</param>
    /// <param name="expireInSeconds">Expiration of the object in seconds.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="KeyAlreadyExistsException">Will be thrown the key already exists and
    ///     <param name="errorIfExists">errorIfExists</param>
    ///     is set.
    /// </exception>
    Task Write<T>(string key, T value, bool errorIfExists, int? expireInSeconds = null);

    /// <summary>
    ///     Write an object into cache
    /// </summary>
    /// <param name="key">Key of the object.</param>
    /// <param name="value">Object to cache.</param>
    /// <param name="errorIfExists">Throw a KeyAlreadyExistsException when the key already exists in the cache.</param>
    /// <param name="expireIn">Expiration of the object.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="KeyAlreadyExistsException">Will be thrown the key already exists and
    ///     <param name="errorIfExists">errorIfExists</param>
    ///     is set.
    /// </exception>
    Task Write<T>(string key, T value, bool errorIfExists, TimeSpan expireIn);

    /// <summary>
    ///     Reads an object from cache
    /// </summary>
    /// <param name="key"></param>
    /// <param name="ignored">This value is ignored</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidTypeException">Will be thrown if the type does not match</exception>
    Task<T?> Read<T>(string key, T? ignored = null) where T : struct;

    /// <summary>
    ///     Reads an object from cache
    /// </summary>
    /// <param name="key"></param>
    /// <param name="ignored">This value is ignored</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidTypeException">Will be thrown if the type does not match</exception>
    Task<T?> Read<T>(string key, ClassConstraint<T>? ignored = null) where T : class?;

    /// <summary>
    ///     Reads an object from cache
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<dynamic?> Read(string key);

    /// <summary>
    ///     Deletes an object from the cache
    /// </summary>
    /// <param name="key"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [Obsolete("Please use the non generic Delete method instead.")]
    Task Delete<T>(string key);

    /// <summary>
    ///     Deletes an object from the cache
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task Delete(string key);

    /// <summary>
    ///     Deletes multiple objects from the cache
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    Task Delete(IEnumerable<string> keys);

    /// <summary>
    ///     Reads a value from the cache if it exists, else execute the function, write it to the cache and return
    /// </summary>
    /// <param name="key"></param>
    /// <param name="func"></param>
    /// <param name="expireInSeconds"></param>
    /// <param name="ignored">This value is ignored</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidTypeException">Will be thrown if the type does not match</exception>
    Task<T?> ReadOrWrite<T>(string key, Func<T?> func, int? expireInSeconds = null, T? ignored = null) where T : struct;

    /// <summary>
    ///     Reads a value from the cache if it exists, else execute the function, write it to the cache and return
    /// </summary>
    /// <param name="key"></param>
    /// <param name="func"></param>
    /// <param name="expireInSeconds"></param>
    /// <param name="ignored">This value is ignored</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidTypeException">Will be thrown if the type does not match</exception>
    Task<T?> ReadOrWrite<T>(string key, Func<T?> func, int? expireInSeconds = null, ClassConstraint<T>? ignored = null)
        where T : class?;

    /// <summary>
    ///     Reads a value from the cache if it exists, else execute the function, write it to the cache and return
    /// </summary>
    /// <param name="key"></param>
    /// <param name="func"></param>
    /// <param name="expireInSeconds"></param>
    /// <param name="ignored">This value is ignored</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidTypeException">Will be thrown if the type does not match</exception>
    Task<T?> ReadOrWrite<T>(string key, Func<Task<T?>> func, int? expireInSeconds = null, T? ignored = null)
        where T : struct;

    /// <summary>
    ///     Reads a value from the cache if it exists, else execute the function, write it to the cache and return
    /// </summary>
    /// <param name="key"></param>
    /// <param name="func"></param>
    /// <param name="expireInSeconds"></param>
    /// <param name="ignored">This value is ignored</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidTypeException">Will be thrown if the type does not match</exception>
    Task<T?> ReadOrWrite<T>(string key, Func<Task<T?>> func, int? expireInSeconds = null,
        ClassConstraint<T>? ignored = null) where T : class?;

    /// <summary>
    ///     Reads a value from the cache if it exists, else execute the function, write it to the cache and return
    /// </summary>
    /// <param name="key"></param>
    /// <param name="func"></param>
    /// <param name="expireIn"></param>
    /// <param name="ignored">This value is ignored</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidTypeException">Will be thrown if the type does not match</exception>
    Task<T?> ReadOrWrite<T>(string key, Func<T?> func, TimeSpan expireIn, T? ignored = null) where T : struct;

    /// <summary>
    ///     Reads a value from the cache if it exists, else execute the function, write it to the cache and return
    /// </summary>
    /// <param name="key"></param>
    /// <param name="func"></param>
    /// <param name="expireIn"></param>
    /// <param name="ignored">This value is ignored</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidTypeException">Will be thrown if the type does not match</exception>
    Task<T?> ReadOrWrite<T>(string key, Func<T?> func, TimeSpan expireIn, ClassConstraint<T>? ignored = null)
        where T : class?;

    /// <summary>
    ///     Reads a value from the cache if it exists, else execute the function, write it to the cache and return
    /// </summary>
    /// <param name="key"></param>
    /// <param name="func"></param>
    /// <param name="expireIn"></param>
    /// <param name="ignored">This value is ignored</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidTypeException">Will be thrown if the type does not match</exception>
    Task<T?> ReadOrWrite<T>(string key, Func<Task<T?>> func, TimeSpan expireIn, T? ignored = null) where T : struct;

    /// <summary>
    ///     Reads a value from the cache if it exists, else execute the function, write it to the cache and return
    /// </summary>
    /// <param name="key"></param>
    /// <param name="func"></param>
    /// <param name="expireIn"></param>
    /// <param name="ignored">This value is ignored</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidTypeException">Will be thrown if the type does not match</exception>
    Task<T?> ReadOrWrite<T>(string key, Func<Task<T?>> func, TimeSpan expireIn, ClassConstraint<T>? ignored = null)
        where T : class?;

    /// <summary>
    ///     Returns a list of all keys and optionally starting with the prefix
    /// </summary>
    /// <param name="prefix"></param>
    /// <returns>An array of keys</returns>
    Task<IEnumerable<string>> ListKeys(string? prefix = null);
}