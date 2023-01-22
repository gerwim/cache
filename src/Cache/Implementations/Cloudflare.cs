using System;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl.Http;
using GerwimFeiken.Cache.Exceptions;
using GerwimFeiken.Cache.Utils;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GerwimFeiken.Cache.Implementations
{
    public class Cloudflare : BaseCache
    {
        private readonly string _apiUrl;
        private readonly string _apiToken;
        private readonly int _expirationTtl;
        
        public Cloudflare(IConfiguration configuration)
        {
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));

            var accountId = configuration.GetRequiredValue("GerwimFeiken.Cache:Cloudflare:AccountId");
            var namespaceId = configuration.GetRequiredValue("GerwimFeiken.Cache:Cloudflare:NamespaceId");
            
            _apiUrl = $"https://api.cloudflare.com/client/v4/accounts/{accountId}/storage/kv/namespaces/{namespaceId}";
            _apiToken = configuration.GetRequiredValue("GerwimFeiken.Cache:Cloudflare:ApiToken");

            try
            {
                _expirationTtl = string.IsNullOrWhiteSpace(configuration["GerwimFeiken.Cache:DefaultExpirationTtl"])
                    ? 86400
                    : Convert.ToInt32(configuration["GerwimFeiken.Cache:DefaultExpirationTtl"]);
            }
            catch
            {
                _expirationTtl = 86400;
            }
        }
        
        protected override async Task WriteImplementation<T>(string key, T value, int? expireInSeconds)
        {
            string json = JsonConvert.SerializeObject(value, settings: new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            await WriteToCloudflare(key, json, expireInSeconds);
        }
        
        protected override async Task<T> ReadImplementation<T>(string key)
        {
            string value = null;
            try
            {
                value = await $"{_apiUrl}/values/{key}"
                    .WithOAuthBearerToken(_apiToken)
                    .GetStringAsync();
            }
            catch (FlurlHttpException ex)
            {
                if (ex.Message.Contains("404 (Not Found)"))
                {
                    // nothing
                }
                else
                {
                    throw;
                }
            }

            if (value == null) return default;

            T obj = JsonConvert.DeserializeObject<T>(value);
            return obj;
        }
        
        private async Task WriteToCloudflare(string key, string value, int? expireInSeconds)
        {
            await
                $"{_apiUrl}/values/{key}?expiration_ttl={expireInSeconds?.ToString() ?? _expirationTtl.ToString()}"
                    .WithOAuthBearerToken(_apiToken)
                    .PutAsync(new StringContent(value));
        }
    }
}