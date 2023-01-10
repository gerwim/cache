using System;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GerwimFeiken.Cache.Implementations
{
    public class Cloudflare : BaseCache
    {
        private readonly IConfiguration _configuration;
        private readonly int _expirationTtl;
        
        public Cloudflare(IConfiguration configuration)
        {
            _configuration = configuration;

            try
            {
                _expirationTtl = string.IsNullOrWhiteSpace(configuration["GerwimFeiken.Cache:ExpirationTtl"])
                    ? 86400
                    : Convert.ToInt32(configuration["GerwimFeiken.Cache:ExpirationTtl"]);
            }
            catch
            {
                _expirationTtl = 86400;
            }
        }
        
        protected override async Task WriteImplementation<T>(string key, T value, int? expireInSeconds)
        {
            string json = JsonConvert.SerializeObject(value);
            await WriteToCloudflare(key, json, expireInSeconds);
        }
        
        protected override async Task<T> ReadImplementation<T>(string key)
        {
            string value = null;
            try
            {
                value = await $"{_configuration["GerwimFeiken.Cache:Cloudflare:KVUrl"]}/values/{key}"
                    .WithOAuthBearerToken(_configuration["GerwimFeiken.Cache:Cloudflare:ApiToken"])
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
                $"{_configuration["GerwimFeiken.Cache:Cloudflare:KVUrl"]}/values/{key}?expiration_ttl={expireInSeconds?.ToString() ?? _expirationTtl.ToString()}"
                    .WithOAuthBearerToken(_configuration["GerwimFeiken.Cache:Cloudflare:ApiToken"])
                    .PutAsync(new StringContent(value));
        }
    }
}