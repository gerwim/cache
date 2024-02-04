using GerwimFeiken.Cache;

namespace Cache.Generators.Sample;

// This code will not compile until you build the project with the Source Generators

public partial class SampleEntity
{
    public int Id { get; } = 42;
    [CacheKey]
    public string? Name { get; } = "Sample";
}