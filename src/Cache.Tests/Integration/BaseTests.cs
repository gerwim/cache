using FluentAssertions;
using GerwimFeiken.Cache;
using GerwimFeiken.Cache.Options;

namespace Cache.Tests.Integration;

public abstract class BaseTests<T> where T : BaseCache
{
    private readonly IOptions _options;
    protected BaseTests(IOptions options)
    {
        _options = options;
    }

    [Fact] public async Task ReadKeyShouldReturnNull()
    {
        // Arrange 
        var sut = (T) Activator.CreateInstance(typeof(T), _options)!;
        // Act
        var key = await sut.Read<string>("test");
        // Assert
        key.Should().BeNull();
    }
    
    [Fact] public async Task WriteAndReadKey()
    {
        // Arrange 
        var sut = (T) Activator.CreateInstance(typeof(T), _options)!;
        // Act
        await sut.Write<string>("writeAndRead", "unitTest");
        var key = await sut.Read<string>("writeAndRead");
        // Assert
        key.Should().Be("unitTest");
    }
    
    [Fact] public async Task WriteAndReadKey_Expired()
    {
        // Arrange 
        var sut = (T) Activator.CreateInstance(typeof(T), _options)!;
        // Act
        await sut.Write<string>("writeAndRead", "unitTest", 60);
        var key1 = await sut.Read<string>("writeAndRead");
        Thread.Sleep(61000);
        var key2 = await sut.Read<string>("writeAndRead");
        // Assert
        key1.Should().Be("unitTest");
        key2.Should().BeNull();
    }
    
    [Fact] public async Task WriteAndDeleteAndReadKey()
    {
        // Arrange 
        var sut = (T) Activator.CreateInstance(typeof(T), _options)!;
        // Act
        await sut.Write<string>("writeAndDelete", "unitTest");
        await sut.Delete<string>("writeAndDelete");
        var key = await sut.Read<string>("writeAndDelete");
        // Assert
        key.Should().BeNull();
    }
}