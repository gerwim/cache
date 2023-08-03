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

        public Task Write<T>(string key, T value, bool errorIfExists, int? expireInSeconds = null)
        {
            // Convert the key
            string encodedKey = $"{typeof(T)}-{key}".ComputeSha256Hash();
            
            return WriteImplementation(encodedKey, value, errorIfExists, expireInSeconds);
        }

        public async Task<T?> Read<T>(string key)
        {
            // Convert the key
            string encodedKey = $"{typeof(T)}-{key}".ComputeSha256Hash();

            return (await ReadImplementation<T?>(encodedKey)).Value;
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