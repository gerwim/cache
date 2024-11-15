using FluentAssertions;
using GerwimFeiken.Cache.InMemory;
using GerwimFeiken.Cache.InMemory.Options;

namespace Cache.Tests.Integration;

public class CacheStampedeTests
{
    [Fact]
    public async Task MultipleRequestsSingleExecution()
    {
        // Arrange 
        var sut = new InMemoryCache(new InMemoryOptions());
        var calls = 0;
        List<Task> tasks = [];
        for (int i = 0; i < 5; i++)
        {
            tasks.Add(sut.ReadOrWrite(nameof(MultipleRequestsSingleExecution), () =>
            {
                Interlocked.Increment(ref calls);
                return Task.FromResult<string?>("result");
            }));
        }

        // Act
        await Task.WhenAll(tasks).ConfigureAwait(false);

        // Assert
        calls.Should().Be(1);
    }
}