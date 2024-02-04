using GerwimFeiken.Cache;
using GerwimFeiken.Cache.InMemory;
using GerwimFeiken.Cache.InMemory.Options;

namespace Cache.Generators.Sample;

public class TestCache
{
    public TestCache()
    {
        var cache = new SampleEntityCache(new InMemoryCache(new InMemoryOptions { DefaultExpirationTtl = 86400 }));

        var a = new SampleEntity();
        // cache.Write(a, 1, "b");
    }
}