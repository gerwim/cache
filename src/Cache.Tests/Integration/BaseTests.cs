using Cache.Tests.Models;
using FluentAssertions;
using GerwimFeiken.Cache;
using GerwimFeiken.Cache.Exceptions;
using GerwimFeiken.Cache.Options;

namespace Cache.Tests.Integration;

public abstract class BaseTests<T> where T : BaseCache
{
    private readonly IOptions _options;

    protected BaseTests(IOptions options)
    {
        _options = options;
    }

    [Fact]
    public async Task ReadKeyShouldReturnNull()
    {
        // Arrange 
        var sut = (T)Activator.CreateInstance(typeof(T), _options)!;
        // Act
        var key = await sut.Read<string>("test");
        // Assert
        key.Should().BeNull();
    }

    [Fact]
    public async Task WriteAndReadKey()
    {
        // Arrange 
        var sut = (T)Activator.CreateInstance(typeof(T), _options)!;
        // Act
        await sut.Write<string>("writeAndRead", "unitTest");
        var key = await sut.Read<string>("writeAndRead");
        // Assert
        key.Should().Be("unitTest");
    }
    
    [Fact]
    public async Task WriteAndReadKey_ComplexObject()
    {
        // Arrange 
        var sut = (T)Activator.CreateInstance(typeof(T), _options)!;
        var complexObject = new ComplexObject
        {
            StringValue = "String",
            GuidValue = Guid.Empty,
            DateTimeValue = DateTime.MinValue,
        };
        // Act
        await sut.Write(nameof(WriteAndReadKey_ComplexObject), complexObject);
        var key = await sut.Read<ComplexObject>(nameof(WriteAndReadKey_ComplexObject));
        // Assert
        key.Should().Be(complexObject);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task WriteAndReadKey_Bool(bool value)
    {
        // Arrange 
        var sut = (T)Activator.CreateInstance(typeof(T), _options)!;
        // Act
        await sut.Write($"{nameof(WriteAndReadKey_Bool)}_{value}", value);
        var key = await sut.Read<bool>($"{nameof(WriteAndReadKey_Bool)}_{value}");
        // Assert
        key.Should().Be(value);
    }

    [Fact]
    public async Task WriteAndReadKey_Expired()
    {
        // Arrange 
        var sut = (T)Activator.CreateInstance(typeof(T), _options)!;
        // Act
        await sut.Write<string>(nameof(WriteAndReadKey_Expired), "unitTest", 60);
        var key1 = await sut.Read<string>(nameof(WriteAndReadKey_Expired));
        await Task.Delay(61000);
        var key2 = await sut.Read<string>(nameof(WriteAndReadKey_Expired));
        // Assert
        key1.Should().Be("unitTest");
        key2.Should().BeNull();
    }

    [Fact]
    public async Task WriteAndDeleteAndReadKey()
    {
        // Arrange 
        var sut = (T)Activator.CreateInstance(typeof(T), _options)!;
        // Act
        await sut.Write<string>("writeAndDelete", "unitTest");
        await sut.Delete<string>("writeAndDelete");
        var key = await sut.Read<string>("writeAndDelete");
        // Assert
        key.Should().BeNull();
    }
    
    [Fact]
    public async Task WriteIfNotExistsTrue_String()
    {
        // Arrange 
        var key = Guid.NewGuid().ToString(); // we set a random value because it's an integration test. Two tests after another might conflict with some providers
        var sut = (T) Activator.CreateInstance(typeof(T), _options)!;
        await sut.Write<string>(key, "unitTest", true, 60);
        // Act
        var act = async () => await sut.Write<string>(key, "unitTest", true, 60);
        // Assert
        await act.Should().ThrowAsync<KeyAlreadyExistsException>();
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task WriteIfNotExistsTrue_Bool(bool value)
    {
        // Arrange 
        var key = Guid.NewGuid().ToString(); // we set a random value because it's an integration test. Two tests after another might conflict with some providers
        var sut = (T) Activator.CreateInstance(typeof(T), _options)!;
        await sut.Write(key, value, true, 60);
        // Act
        var act = async () => await sut.Write(key, value, true, 60);
        // Assert
        await act.Should().ThrowAsync<KeyAlreadyExistsException>();
    }
    
    [Fact]
    public async Task WriteIfNotExistsTrue_NullableBool()
    {
        // Arrange
        var key = Guid.NewGuid().ToString(); // we set a random value because it's an integration test. Two tests after another might conflict with some providers
        var sut = (T) Activator.CreateInstance(typeof(T), _options)!;
        await sut.Write<bool?>(key, true, true, 60);
        // Act
        var act = async () => await sut.Write<bool?>(key, true, true, 60);
        // Assert
        await act.Should().ThrowAsync<KeyAlreadyExistsException>();
    }
    
    [Fact]
    public async Task WriteIfNotExistsTrue_Expired()
    {
        // Arrange 
        var sut = (T) Activator.CreateInstance(typeof(T), _options)!;
        await sut.Write<string>(nameof(WriteIfNotExistsTrue_Expired), "unitTest", false, 60);
        // Act
        await Task.Delay(61000);
        var act = async () => await sut.Write<string>(nameof(WriteIfNotExistsTrue_Expired), "unitTest", true);
        // Assert
        await act.Should().NotThrowAsync<KeyAlreadyExistsException>();
    }
    
    [Fact]
    public async Task WriteIfNotExistsFalse()
    {
        // Arrange 
        var sut = (T) Activator.CreateInstance(typeof(T), _options)!;
        await sut.Write<string>(nameof(WriteIfNotExistsFalse), "unitTest", false);
        // Act
        var act = async () => await sut.Write<string>(nameof(WriteIfNotExistsFalse), "unitTest", false);
        // Assert
        await act.Should().NotThrowAsync<KeyAlreadyExistsException>();
    }
}