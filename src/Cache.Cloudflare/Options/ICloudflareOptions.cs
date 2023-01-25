using GerwimFeiken.Cache.Options;

namespace GerwimFeiken.Cache.Cloudflare.Options;

public interface ICloudflareOptions : IOptions
{
    public string? ApiToken { get; set; }
    public string? AccountId { get; set; }
    public string? NamespaceId { get; set; }
}