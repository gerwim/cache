using GerwimFeiken.Cache.Cloudflare;
using GerwimFeiken.Cache.Cloudflare.Options;

namespace Cache.Tests.Integration;

public class CloudflareTests : BaseTests<CloudflareCache>
{
    public CloudflareTests() : base(new CloudflareOptions
    {
        ApiToken = null,
        AccountId = null,
        NamespaceId = null
    })
    { }
}