﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GerwimFeiken.Cache.Cloudflare.Models;
using GerwimFeiken.Cache.Cloudflare.Options;
using GerwimFeiken.Cache.Cloudflare.Repositories;
using GerwimFeiken.Cache.Exceptions;
using GerwimFeiken.Cache.Models;
using GerwimFeiken.Cache.Utils;

namespace GerwimFeiken.Cache.Cloudflare
{
    public class CloudflareCache : BaseCache
    {
        private readonly int _expirationTtl;
        private readonly CloudflareApi _cloudflareApi;
        
        public CloudflareCache(ICloudflareOptions options) : base(options)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));

            var accountId = options.GetRequiredValue(x => x.AccountId);
            var namespaceId = options.GetRequiredValue(x => x.NamespaceId);
            
            var apiUrl = $"https://api.cloudflare.com/client/v4/accounts/{accountId}/storage/kv/namespaces/{namespaceId}";
            var apiToken = options.GetRequiredValue(x => x.ApiToken)!;

            _cloudflareApi = new CloudflareApi(apiUrl, apiToken);

            _expirationTtl = options.GetRequiredValue(x => x.DefaultExpirationTtl);
        }
        
        protected override async Task DeleteImplementation(string key)
        {
            var response = await _cloudflareApi.DeleteKey(key).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound)
            {
                throw new DeleteException($"Could not delete from Cloudflare: {await response.Content.ReadAsStringAsync().ConfigureAwait(false)}");
            }
        }

        protected override async Task DeleteImplementation(IEnumerable<string> keys)
        {
            var response = await _cloudflareApi.DeleteKeys(keys).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound)
            {
                throw new DeleteException($"Could not delete from Cloudflare: {await response.Content.ReadAsStringAsync().ConfigureAwait(false)}");
            }
        }

        protected override async Task<IEnumerable<string>> ListKeysImplementation(string? prefix)
        {
            var response = await _cloudflareApi.ListKeys(prefix).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new DeleteException($"Could not list keys from Cloudflare: {await response.Content.ReadAsStringAsync().ConfigureAwait(false)}");
            }
            
            var obj = DeserializeObject<CloudflareListKeysResponse>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            return obj?.Result?.Select(x => x.Name) ?? Array.Empty<string>();
        }

        protected override async Task<ReadResult> ReadImplementation(string key)
        {
            var response = await _cloudflareApi.GetKey(key).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode && response.StatusCode is not HttpStatusCode.NotFound)
            {
                throw new ReadException($"Could not read from Cloudflare: {await response.Content.ReadAsStringAsync().ConfigureAwait(false)}");
            }

            if (response.Content is null || response.StatusCode is HttpStatusCode.NotFound) return ReadResult.Fail(null, ReadReason.KeyDoesNotExist);

            return ReadResult.Ok(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
        }

        protected override async Task<WriteResult> WriteImplementation(string key, string value, int? expireInSeconds)
        {
            if ((expireInSeconds ?? _expirationTtl) < 60)
            {
                throw new WriteException("Expiration should be 60 or greater.");
            }
            
            var response = await _cloudflareApi.WriteKey(key, expireInSeconds ?? _expirationTtl, value).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new WriteException($"Could not write to Cloudflare: {await response.Content.ReadAsStringAsync().ConfigureAwait(false)}");
            }

            return WriteResult.Ok();
        }
        
        protected override async Task<WriteResult> WriteImplementation(string key, string value, bool errorIfExists, int? expireInSeconds)
        {
            if (errorIfExists)
            {
                var result = await ReadImplementation(key).ConfigureAwait(false);
                if (result.OperationStatus is Status.Ok) throw new KeyAlreadyExistsException();
            }

            await WriteImplementation(key, value, expireInSeconds).ConfigureAwait(false);

            return WriteResult.Ok();
        }
    }
}