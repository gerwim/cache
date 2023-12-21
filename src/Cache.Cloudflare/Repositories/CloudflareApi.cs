using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GerwimFeiken.Cache.Cloudflare.Repositories;
public class CloudflareApi
{
    private static readonly HttpClient HttpClient = new();
    private readonly string _apiUrl;
    private readonly string _apiToken;

    public CloudflareApi(string apiUrl, string apiToken)
    {
        _apiUrl = apiUrl;
        _apiToken = apiToken;
    }
    
    public async Task<HttpResponseMessage> GetKey(string keyId)
    {
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri($"{_apiUrl}/values/{keyId}"),
            Method = HttpMethod.Get,
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiToken);

        return await HttpClient.SendAsync(request).ConfigureAwait(false);
    }
    
    public async Task<HttpResponseMessage> ListKeys(string? prefix)
    {
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri($"{_apiUrl}/keys{(!string.IsNullOrEmpty(prefix) ? $"?prefix={prefix}" : null)}"),
            Method = HttpMethod.Get,
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiToken);

        return await HttpClient.SendAsync(request).ConfigureAwait(false);
    }
    
    public async Task<HttpResponseMessage> DeleteKey(string keyId)
    {
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri($"{_apiUrl}/values/{keyId}"),
            Method = HttpMethod.Delete,
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiToken);

        return await HttpClient.SendAsync(request).ConfigureAwait(false);
    }
    
    public async Task<HttpResponseMessage> WriteKey(string keyId, int expirationTtl, string content)
    {
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri($"{_apiUrl}/values/{keyId}?expiration_ttl={expirationTtl}"),
            Method = HttpMethod.Put,
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiToken);

        request.Content = new StringContent(content);

        return await HttpClient.SendAsync(request).ConfigureAwait(false);
    }
}