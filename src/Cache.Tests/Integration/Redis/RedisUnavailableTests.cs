using FluentAssertions;
using GerwimFeiken.Cache.Options;
using GerwimFeiken.Cache.Redis;
using GerwimFeiken.Cache.Redis.Options;
using StackExchange.Redis;

namespace Cache.Tests.Integration.Redis;

public class RedisUnavailableTests
{
    private readonly IOptions _options;
    private readonly IOptions _optionsWithIgnore;

    public RedisUnavailableTests()
    {
        _options = new RedisOptions
        {
            Configuration = "127.0.0.2,abortConnect=false,connectTimeout=1000,connectRetry=0",
            IgnoreTimeouts = false,
        };
        _optionsWithIgnore = new RedisOptions
        {
            Configuration = "127.0.0.2,abortConnect=false,connectTimeout=1000,connectRetry=0",
            IgnoreTimeouts = true,
        };
    }

    [Fact]
    public async Task ReadKeyShouldReturnNull()
    {
        // Arrange 
        var sut = (RedisCache)Activator.CreateInstance(typeof(RedisCache), _optionsWithIgnore)!;
        // Act
        var key = await sut.Read<string>("test").ConfigureAwait(false);
        // Assert
        key.Should().BeNull();
    }

    [Fact]
    public async Task ReadKeyShouldThrowTimeoutException()
    {
        // Arrange 
        var sut = (RedisCache)Activator.CreateInstance(typeof(RedisCache), _options)!;
        // Act
        var act = async () => await sut.Read<string>("test").ConfigureAwait(false);
        // Assert
        await act.Should().ThrowAsync<RedisConnectionException>().ConfigureAwait(false);
    }

    [Fact]
    public async Task WriteKeyShouldThrowTimeoutException()
    {
        // Arrange 
        var sut = (RedisCache)Activator.CreateInstance(typeof(RedisCache), _options)!;
        // Act
        var act = async () => await sut.Write<string>("test", "value").ConfigureAwait(false);
        // Assert
        await act.Should().ThrowAsync<RedisConnectionException>().ConfigureAwait(false);
    }

    [Fact]
    public async Task DeleteKeyShouldThrowTimeoutException()
    {
        // Arrange 
        var sut = (RedisCache)Activator.CreateInstance(typeof(RedisCache), _options)!;
        // Act
        var act = async () => await sut.Delete<string>("test").ConfigureAwait(false);
        // Assert
        await act.Should().ThrowAsync<RedisConnectionException>().ConfigureAwait(false);
    }

    [Fact]
    public async Task WriteAndReadKey()
    {
        // Arrange 
        var sut = (RedisCache)Activator.CreateInstance(typeof(RedisCache), _optionsWithIgnore)!;
        // Act
        await sut.Write<string>("writeAndRead", "unitTest").ConfigureAwait(false);
        var key = await sut.Read<string>("writeAndRead").ConfigureAwait(false);
        // Assert
        key.Should().BeNull();
    }

    [Fact]
    public async Task WriteAndReadKey_Expired()
    {
        // Arrange 
        var sut = (RedisCache)Activator.CreateInstance(typeof(RedisCache), _optionsWithIgnore)!;
        // Act
        await sut.Write<string>("writeAndRead", "unitTest", 60).ConfigureAwait(false);
        var key1 = await sut.Read<string>("writeAndRead").ConfigureAwait(false);
        var key2 = await sut.Read<string>("writeAndRead").ConfigureAwait(false);
        // Assert
        key1.Should().BeNull();
        key2.Should().BeNull();
    }

    [Fact]
    public async Task WriteAndDeleteAndReadKey()
    {
        // Arrange 
        var sut = (RedisCache)Activator.CreateInstance(typeof(RedisCache), _optionsWithIgnore)!;
        // Act
        await sut.Write<string>("writeAndDelete", "unitTest").ConfigureAwait(false);
        await sut.Delete<string>("writeAndDelete").ConfigureAwait(false);
        var key = await sut.Read<string>("writeAndDelete").ConfigureAwait(false);
        // Assert
        key.Should().BeNull();
    }
}