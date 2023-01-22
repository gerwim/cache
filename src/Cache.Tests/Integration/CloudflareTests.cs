using GerwimFeiken.Cache.Implementations;
using GerwimFeiken.Cache.Options;

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