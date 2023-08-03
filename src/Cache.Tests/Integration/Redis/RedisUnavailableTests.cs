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
        var key = await sut.Read<string>("test");
        // Assert
        key.Should().BeNull();
    }

    [Fact]
    public async Task ReadKeyShouldThrowTimeoutException()
    {
        // Arrange 
        var sut = (RedisCache)Activator.CreateInstance(typeof(RedisCache), _options)!;
        // Act
        var act = async () => await sut.Read<string>("test");
        // Assert
        await act.Should().ThrowAsync<RedisConnectionException>();
    }

    [Fact]
    public async Task WriteKeyShouldThrowTimeoutException()
    {
        // Arrange 
        var sut = (RedisCache)Activator.CreateInstance(typeof(RedisCache), _options)!;
        // Act
        var act = async () => await sut.Write<string>("test", "value");
        // Assert
        await act.Should().ThrowAsync<RedisConnectionException>();
    }

    [Fact]
    public async Task DeleteKeyShouldThrowTimeoutException()
    {
        // Arrange 
        var sut = (RedisCache)Activator.CreateInstance(typeof(RedisCache), _options)!;
        // Act
        var act = async () => await sut.Delete<string>("test");
        // Assert
        await act.Should().ThrowAsync<RedisConnectionException>();
    }

    [Fact]
    public async Task WriteAndReadKey()
    {
        // Arrange 
        var sut = (RedisCache)Activator.CreateInstance(typeof(RedisCache), _optionsWithIgnore)!;
        // Act
        await sut.Write<string>("writeAndRead", "unitTest");
        var key = await sut.Read<string>("writeAndRead");
        // Assert
        key.Should().BeNull();
    }

    [Fact]
    public async Task WriteAndReadKey_Expired()
    {
        // Arrange 
        var sut = (RedisCache)Activator.CreateInstance(typeof(RedisCache), _optionsWithIgnore)!;
        // Act
        await sut.Write<string>("writeAndRead", "unitTest", 60);
        var key1 = await sut.Read<string>("writeAndRead");
        var key2 = await sut.Read<string>("writeAndRead");
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
        await sut.Write<string>("writeAndDelete", "unitTest");
        await sut.Delete<string>("writeAndDelete");
        var key = await sut.Read<string>("writeAndDelete");
        // Assert
        key.Should().BeNull();
    }
}