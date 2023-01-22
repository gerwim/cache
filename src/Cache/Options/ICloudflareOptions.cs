namespace GerwimFeiken.Cache.Options;

public interface ICloudflareOptions : IOptions
{
    public string ApiToken { get; set; }
    public string AccountId { get; set; }
    public string NamespaceId { get; set; }
}