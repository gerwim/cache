using GerwimFeiken.Cache.Implementations;
using GerwimFeiken.Cache.Options;

namespace Cache.Tests.Integration;

public class InMemoryTests : BaseTests<InMemoryCache>
{
    public InMemoryTests() : base(new InMemoryOptions())
    { }
}