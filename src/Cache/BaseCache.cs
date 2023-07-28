using System.Threading.Tasks;
using GerwimFeiken.Cache.Utils.Extensions;

namespace GerwimFeiken.Cache
{
    public abstract class BaseCache : ICache
    {
        public Task Write<T>(string key, T value, int? expireInSeconds = null)
        {
            // Convert the key
            string encodedKey = $"{typeof(T)}-{key}".ComputeSha256Hash();
            
            return this.WriteImplementation(encodedKey, value, expireInSeconds);
        }

        public Task Write<T>(string key, T value, bool errorIfExists, int? expireInSeconds = null)
        {
            // Convert the key
            string encodedKey = $"{typeof(T)}-{key}".ComputeSha256Hash();
            
            return this.WriteImplementation(encodedKey, value, errorIfExists, expireInSeconds);
        }

        public Task<T?> Read<T>(string key)
        {
            // Convert the key
            string encodedKey = $"{typeof(T)}-{key}".ComputeSha256Hash();

            return this.ReadImplementation<T?>(encodedKey);
        }
        
        public Task Delete<T>(string key)
        {
            // Convert the key
            string encodedKey = $"{typeof(T)}-{key}".ComputeSha256Hash();
            
            return this.DeleteImplementation(encodedKey);
        }

        protected abstract Task DeleteImplementation(string key);
        protected abstract Task<T?> ReadImplementation<T>(string key);
        protected abstract Task WriteImplementation<T>(string key, T value, int? expireInSeconds);
        protected abstract Task WriteImplementation<T>(string key, T value, bool errorIfExists, int? expireInSeconds);
    }
}