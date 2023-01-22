namespace GerwimFeiken.Cache.Options;

public class CloudflareOptions : Options, ICloudflareOptions
{
    public string ApiToken { get; set; }
    public string AccountId { get; set; }
    public string NamespaceId { get; set; }
}