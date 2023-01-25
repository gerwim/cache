using GerwimFeiken.Cache.InMemory;
using GerwimFeiken.Cache.InMemory.Options;

namespace Cache.Tests.Integration;

public class InMemoryTests : BaseTests<InMemoryCache>
{
    public InMemoryTests() : base(new InMemoryOptions())
    { }
}