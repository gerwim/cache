using Cache.Tests.Integration.Fixtures;
using GerwimFeiken.Cache.Redis;
using GerwimFeiken.Cache.Redis.Options;

namespace Cache.Tests.Integration.Redis;

public class RedisTests : BaseTests<RedisCache>, IClassFixture<RedisFixture>
{
    public RedisTests() : base(new RedisOptions
    {
        Configuration = "127.0.0.1"
    })
    { }
}