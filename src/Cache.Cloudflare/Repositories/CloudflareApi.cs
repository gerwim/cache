using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace GerwimFeiken.Cache.Cloudflare.Repositories;

[Headers("Authorization: Bearer")]
public interface ICloudflareApi
{
    [Get("/values/{id}")]
    public Task<ApiResponse<string>> GetKey([AliasAs("id")] string keyId);
    
    [Delete("/values/{id}")]
    public Task<ApiResponse<CloudflareResponse>> DeleteKey([AliasAs("id")] string keyId);
    
    [Put("/values/{id}")]
    public Task<ApiResponse<string>> WriteKey(
        [AliasAs("id")] string keyId,
        [Query] [AliasAs("expiration_ttl")] int expirationTtl,
        [Body] string content);
}

public record CloudflareResponse
{
    public bool Success { get; set; }
    public IEnumerable<string>? Errors { get; set; }
    public IEnumerable<string>? Messages { get; set; }
}