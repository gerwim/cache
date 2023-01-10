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

        public Task<T> Read<T>(string key)
        {
            // Convert the key
            string encodedKey = $"{typeof(T)}-{key}".ComputeSha256Hash();

            return this.ReadImplementation<T>(encodedKey);
        }

        protected abstract Task<T> ReadImplementation<T>(string key);
        protected abstract Task WriteImplementation<T>(string key, T value, int? expireInSeconds);
    }
}