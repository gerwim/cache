using FluentAssertions;
using GerwimFeiken.Cache.Implementations;
using Microsoft.Extensions.Configuration;

namespace Cache.Tests.Integration;

public class CloudflareTests
{
    private readonly IConfiguration _configuration;
    public CloudflareTests()
    {
        var myConfiguration = new Dictionary<string, string>
        {
            {"GerwimFeiken.Cache:Cloudflare:ApiToken", ""},
            {"GerwimFeiken.Cache:Cloudflare:AccountId", ""},
            {"GerwimFeiken.Cache:Cloudflare:NamespaceId", ""}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(myConfiguration)
            .Build();
    }

    [Fact] public async Task Cloudflare_ReadKeyShouldReturnNull()
    {
        // Arrange 
        var sut = new Cloudflare(_configuration);
        // Act
        var key = await sut.Read<string>("test");
        // Assert
        key.Should().BeNull();
    }
    
    [Fact] public async Task Cloudflare_WriteAndReadKey()
    {
        // Arrange 
        var sut = new Cloudflare(_configuration);
        // Act
        await sut.Write<string>("writeAndRead", "unitTest");
        var key = await sut.Read<string>("writeAndRead");
        // Assert
        key.Should().Be("unitTest");
    }
    
    [Fact] public async Task Cloudflare_WriteAndReadKey_Expired()
    {
        // Arrange 
        var sut = new Cloudflare(_configuration);
        // Act
        await sut.Write<string>("writeAndRead", "unitTest", 60);
        var key1 = await sut.Read<string>("writeAndRead");
        Thread.Sleep(61000);
        var key2 = await sut.Read<string>("writeAndRead");
        // Assert
        key1.Should().Be("unitTest");
        key2.Should().BeNull();
    }
    
    [Fact] public async Task Cloudflare_WriteAndDeleteAndReadKey()
    {
        // Arrange 
        var sut = new Cloudflare(_configuration);
        // Act
        await sut.Write<string>("writeAndDelete", "unitTest");
        await sut.Delete<string>("writeAndDelete");
        var key = await sut.Read<string>("writeAndDelete");
        // Assert
        key.Should().BeNull();
    }
}