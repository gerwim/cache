namespace GerwimFeiken.Cache.Cloudflare.Options;

public class CloudflareOptions : Cache.Options.Options, ICloudflareOptions
{
    public string? ApiToken { get; set; }
    public string? AccountId { get; set; }
    public string? NamespaceId { get; set; }
}