using System;
using System.Net;
using System.Threading.Tasks;
using GerwimFeiken.Cache.Exceptions;
using GerwimFeiken.Cache.Options;
using GerwimFeiken.Cache.Repositories;
using GerwimFeiken.Cache.Utils;
using Newtonsoft.Json;
using Refit;

namespace GerwimFeiken.Cache.Implementations
{
    public class CloudflareCache : BaseCache
    {
        private readonly int _expirationTtl;
        private readonly ICloudflareApi _cloudflareApi;
        
        public CloudflareCache(ICloudflareOptions options)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));

            var accountId = options.GetRequiredValue(x => x.AccountId);
            var namespaceId = options.GetRequiredValue(x => x.NamespaceId);
            
            var apiUrl = $"https://api.cloudflare.com/client/v4/accounts/{accountId}/storage/kv/namespaces/{namespaceId}";
            var apiToken = options.GetRequiredValue(x => x.ApiToken);
            
            _cloudflareApi = RestService.For<ICloudflareApi>(apiUrl, new RefitSettings
            {
                AuthorizationHeaderValueGetter = () => Task.FromResult(apiToken)
            });

            _expirationTtl = options.GetRequiredValue(x => x.DefaultExpirationTtl);
        }
        
        protected override async Task DeleteImplementation(string key)
        {
            var response = await _cloudflareApi.DeleteKey(key);

            if (response.Error is not null && response.StatusCode != HttpStatusCode.NotFound)
            {
                throw new DeleteException("Could not delete from Cloudflare, see inner exception for more details.", response.Error);
            }
        }

        protected override async Task WriteImplementation<T>(string key, T value, int? expireInSeconds)
        {
            if ((expireInSeconds ?? _expirationTtl) < 60)
            {
                throw new WriteException("Expiration should be 60 or greater.");
            }
            
            string json = JsonConvert.SerializeObject(value, settings: new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            var response = await _cloudflareApi.WriteKey(key, expireInSeconds ?? _expirationTtl, json);
            if (response.Error is not null)
            {
                throw new WriteException("Could not write to Cloudflare, see inner exception for more details.", response.Error);
            }
        }
        
        protected override async Task<T> ReadImplementation<T>(string key)
        {
            var response = await _cloudflareApi.GetKey(key);

            if (response.Error is not null && response.StatusCode != HttpStatusCode.NotFound)
            {
                throw new ReadException("Could not read from Cloudflare, see inner exception for more details.", response.Error);
            }

            if (response.Content == null) return default;

            T obj = JsonConvert.DeserializeObject<T>(response.Content);
            return obj;
        }
    }
}