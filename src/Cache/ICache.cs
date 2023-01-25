using System.Threading.Tasks;

namespace GerwimFeiken.Cache
{
    public interface ICache
    {
        /// <summary>
        /// Write an object into cache
        /// </summary>
        /// <param name="key">Key of the object.</param>
        /// <param name="value">Object to cache.</param>
        /// <param name="expireInSeconds">Expiration of the object in seconds.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task Write<T>(string key, T value, int? expireInSeconds = null);
        /// <summary>
        /// Reads an object from cache
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T?> Read<T>(string key);

        /// <summary>
        /// Deletes an object from the cache
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task Delete<T>(string key);
    }
}