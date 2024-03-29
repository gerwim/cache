﻿using Cache.Tests.Models;
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
    public async Task DeleteMultipleKeys()
    {
        // Arrange 
        var key = nameof(DeleteMultipleKeys);
        var sut = (T)Activator.CreateInstance(typeof(T), _options)!;
        await sut.Write<string>($"{key}1", "unitTest").ConfigureAwait(false);
        await sut.Write<string>($"{key}2", "unitTest").ConfigureAwait(false);
        await sut.Write<string>($"{key}3", "unitTest").ConfigureAwait(false);

        // Act
        var act = async () => await sut.Delete([$"{key}1", $"{key}2", $"{key}3"]).ConfigureAwait(false);
        
        // Assert
        await act.Should().NotThrowAsync().ConfigureAwait(false);
    }

    [Fact]
    public async Task ReadKeyShouldReturnNull_ReferenceType()
    {
        // Arrange 
        var sut = (T)Activator.CreateInstance(typeof(T), _options)!;
        // Act
        var key = await sut.Read<string>("test").ConfigureAwait(false);
        // Assert
        key.Should().BeNull();
    }
    
    [Fact]
    public async Task ReadKeyShouldReturnNull_ValueType()
    {
        // Arrange 
        var sut = (T)Activator.CreateInstance(typeof(T), _options)!;
        var key = nameof(ReadKeyShouldReturnNull_ValueType);
        // Act
        var result = await sut.Read<int>(key).ConfigureAwait(false);
        // Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task ReadOrWrite()
    {
        // Arrange 
        var sut = (T)Activator.CreateInstance(typeof(T), _options)!;
        var key = nameof(ReadOrWrite);
        // Act
        var result1 = await sut.ReadOrWrite<string>(key, () => "unitTest").ConfigureAwait(false);
        var result2 = await sut.Read<string>(key).ConfigureAwait(false);
        // Assert
        result1.Should().Be("unitTest");
        result2.Should().Be("unitTest");
    }
    
    [Fact]
    public async Task ReadOrWrite_Async()
    {
        // Arrange 
        var sut = (T)Activator.CreateInstance(typeof(T), _options)!;
        var key = nameof(ReadOrWrite_Async);
        // Act
        var result1 = await sut.ReadOrWrite(key, async () => await Task.FromResult("unitTest").ConfigureAwait(false)).ConfigureAwait(false);
        var result2 = await sut.Read<string>(key).ConfigureAwait(false);
        // Assert
        result1.Should().Be("unitTest");
        result2.Should().Be("unitTest");
    }
    
    [Fact]
    public async Task ReadOrWrite_TimeSpan()
    {
        // Arrange 
        var sut = (T)Activator.CreateInstance(typeof(T), _options)!;
        var key = nameof(ReadOrWrite_TimeSpan);
        // Act
        var result1 = await sut.ReadOrWrite<string>(key, () => "unitTest", TimeSpan.MaxValue).ConfigureAwait(false);
        var result2 = await sut.Read<string>(key).ConfigureAwait(false);
        // Assert
        result1.Should().Be("unitTest");
        result2.Should().Be("unitTest");
    }
    
    [Fact]
    public async Task ReadOrWrite_TimeSpan_Async()
    {
        // Arrange 
        var sut = (T)Activator.CreateInstance(typeof(T), _options)!;
        var key = nameof(ReadOrWrite_TimeSpan_Async);
        // Act
        var result1 = await sut.ReadOrWrite(nameof(ReadOrWrite_TimeSpan_Async), async () => await Task.FromResult("unitTest").ConfigureAwait(false), TimeSpan.MaxValue).ConfigureAwait(false);
        var result2 = await sut.Read<string>(key).ConfigureAwait(false);
        // Assert
        result1.Should().Be("unitTest");
        result2.Should().Be("unitTest");
    }

    [Fact]
    public async Task WriteAndReadKey()
    {
        // Arrange 
        var sut = (T)Activator.CreateInstance(typeof(T), _options)!;
        // Act
        await sut.Write<string>("writeAndRead", "unitTest").ConfigureAwait(false);
        var key = await sut.Read<string>("writeAndRead").ConfigureAwait(false);
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
        var key = nameof(WriteAndReadKey_ComplexObject);
        
        // Act
        await sut.Write(key, complexObject).ConfigureAwait(false);
        var result = await sut.Read<ComplexObject>(key).ConfigureAwait(false);
        
        // Assert
        result.Should().Be(complexObject);
    }
    
    [Fact]
    public async Task WriteAndReadKey_ComplexObject_FromInterface()
    {
        // Arrange 
        var sut = (T)Activator.CreateInstance(typeof(T), _options)!;
        var complexObject = new ComplexObject
        {
            StringValue = "String",
            GuidValue = Guid.Empty,
            DateTimeValue = DateTime.MinValue,
        };
        var key = nameof(WriteAndReadKey_ComplexObject);
        
        // Act
        await sut.Write(key, complexObject).ConfigureAwait(false);
        var result = await sut.Read<IComplexObject>(key).ConfigureAwait(false);
        
        // Assert
        result.Should().Be(complexObject);
    }
    
    [Fact]
    public async Task WriteAndReadKey_ComplexObject_FromInterface_InIEnumerable()
    {
        // Arrange 
        var sut = (T)Activator.CreateInstance(typeof(T), _options)!;
        var complexObject = new ComplexObject
        {
            StringValue = "String",
            GuidValue = Guid.Empty,
            DateTimeValue = DateTime.MinValue,
        };
        IEnumerable<IComplexObject> enumerable = [complexObject];
        var type = enumerable.GetType();
        var key = nameof(WriteAndReadKey_ComplexObject);
        
        // Act
        await sut.Write(key, enumerable).ConfigureAwait(false);
        var result = await sut.Read<IEnumerable<IComplexObject>>(key).ConfigureAwait(false);
        
        // Assert
        result.First().Should().Be(complexObject);
    }
    
    [Fact]
    public async Task WriteAndReadKey_NonPublicSetters()
    {
        // Arrange 
        var sut = (T)Activator.CreateInstance(typeof(T), _options)!;
        var key = nameof(WriteAndReadKey_NonPublicSetters);
        var nonPublic = new NonPublic("private", "init");
        
        // Act
        await sut.Write(key, nonPublic).ConfigureAwait(false);
        var result = await sut.Read<NonPublic>(key).ConfigureAwait(false);
        
        // Assert
        result.Should().Be(nonPublic);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task WriteAndReadKey_Bool(bool value)
    {
        // Arrange 
        var sut = (T)Activator.CreateInstance(typeof(T), _options)!;
        // Act
        await sut.Write($"{nameof(WriteAndReadKey_Bool)}_{value}", value).ConfigureAwait(false);
        var key = await sut.Read<bool>($"{nameof(WriteAndReadKey_Bool)}_{value}").ConfigureAwait(false);
        // Assert
        key.Should().Be(value);
    }
    
    [Fact]
    public async Task WriteAndReadKey_Dynamic()
    {
        // Arrange
        var key = nameof(WriteAndReadKey_Dynamic);
        var sut = (T)Activator.CreateInstance(typeof(T), _options)!;
        
        // Act
        await sut.Write($"{key}", "someValue").ConfigureAwait(false);
        var result = await sut.Read($"{key}").ConfigureAwait(false);
        
        // Assert
        Assert.Equal("someValue", result);
    }

    [Fact]
    public async Task WriteAndReadKey_Expired()
    {
        // Arrange 
        var sut = (T)Activator.CreateInstance(typeof(T), _options)!;
        // Act
        await sut.Write<string>(nameof(WriteAndReadKey_Expired), "unitTest", 60).ConfigureAwait(false);
        var key1 = await sut.Read<string>(nameof(WriteAndReadKey_Expired)).ConfigureAwait(false);
        await Task.Delay(61000).ConfigureAwait(false);
        var key2 = await sut.Read<string>(nameof(WriteAndReadKey_Expired)).ConfigureAwait(false);
        // Assert
        key1.Should().Be("unitTest");
        key2.Should().BeNull();
    }
    
    [Fact]
    public async Task WriteAndReadKey_DifferentType()
    {
        // Arrange 
        var sut = (T)Activator.CreateInstance(typeof(T), _options)!;
        await sut.Write<string>(nameof(WriteAndReadKey_DifferentType), "unitTest", 60).ConfigureAwait(false);
        
        // Act
        var act = async () => await sut.Read<bool>(nameof(WriteAndReadKey_DifferentType)).ConfigureAwait(false);

        // Assert
        await act.Should().ThrowAsync<InvalidTypeException>().ConfigureAwait(false);
    }
    
    [Fact]
    public async Task WriteAndReadKey_DifferentNullability()
    {
        // Arrange 
        var sut = (T)Activator.CreateInstance(typeof(T), _options)!;
        const int expectedResult = 10;
        var key = nameof(WriteAndReadKey_DifferentNullability);
        await sut.Write<int?>(key, expectedResult, 60).ConfigureAwait(false);
        
        // Act
        var result = await sut.Read<int>(key).ConfigureAwait(false);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public async Task WriteAndDeleteAndReadKey()
    {
        // Arrange 
        var sut = (T)Activator.CreateInstance(typeof(T), _options)!;
        // Act
        await sut.Write<string>("writeAndDelete", "unitTest").ConfigureAwait(false);
        await sut.Delete("writeAndDelete").ConfigureAwait(false);
        var key = await sut.Read<string>("writeAndDelete").ConfigureAwait(false);
        // Assert
        key.Should().BeNull();
    }
    
    [Fact]
    public async Task WriteIfNotExistsTrue_String()
    {
        // Arrange 
        var key = Guid.NewGuid().ToString(); // we set a random value because it's an integration test. Two tests after another might conflict with some providers
        var sut = (T) Activator.CreateInstance(typeof(T), _options)!;
        await sut.Write<string>(key, "unitTest", true, 60).ConfigureAwait(false);
        // Act
        var act = async () => await sut.Write<string>(key, "unitTest", true, 60).ConfigureAwait(false);
        // Assert
        await act.Should().ThrowAsync<KeyAlreadyExistsException>().ConfigureAwait(false);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task WriteIfNotExistsTrue_Bool(bool value)
    {
        // Arrange 
        var key = Guid.NewGuid().ToString(); // we set a random value because it's an integration test. Two tests after another might conflict with some providers
        var sut = (T) Activator.CreateInstance(typeof(T), _options)!;
        await sut.Write(key, value, true, 60).ConfigureAwait(false);
        // Act
        var act = async () => await sut.Write(key, value, true, 60).ConfigureAwait(false);
        // Assert
        await act.Should().ThrowAsync<KeyAlreadyExistsException>().ConfigureAwait(false);
    }
    
    [Fact]
    public async Task WriteIfNotExistsTrue_NullableBool()
    {
        // Arrange
        var key = Guid.NewGuid().ToString(); // we set a random value because it's an integration test. Two tests after another might conflict with some providers
        var sut = (T) Activator.CreateInstance(typeof(T), _options)!;
        await sut.Write<bool?>(key, true, true, 60).ConfigureAwait(false);
        // Act
        var act = async () => await sut.Write<bool?>(key, true, true, 60).ConfigureAwait(false);
        // Assert
        await act.Should().ThrowAsync<KeyAlreadyExistsException>().ConfigureAwait(false);
    }
    
    [Fact]
    public async Task WriteIfNotExistsTrue_Expired()
    {
        // Arrange 
        var sut = (T) Activator.CreateInstance(typeof(T), _options)!;
        await sut.Write<string>(nameof(WriteIfNotExistsTrue_Expired), "unitTest", false, 60).ConfigureAwait(false);
        // Act
        await Task.Delay(61000).ConfigureAwait(false);
        var act = async () => await sut.Write<string>(nameof(WriteIfNotExistsTrue_Expired), "unitTest", true).ConfigureAwait(false);
        // Assert
        await act.Should().NotThrowAsync<KeyAlreadyExistsException>().ConfigureAwait(false);
    }
    
    [Fact]
    public async Task WriteIfNotExistsFalse()
    {
        // Arrange 
        var sut = (T) Activator.CreateInstance(typeof(T), _options)!;
        await sut.Write<string>(nameof(WriteIfNotExistsFalse), "unitTest", false).ConfigureAwait(false);
        // Act
        var act = async () => await sut.Write<string>(nameof(WriteIfNotExistsFalse), "unitTest", false).ConfigureAwait(false);
        // Assert
        await act.Should().NotThrowAsync<KeyAlreadyExistsException>().ConfigureAwait(false);
    }

    [Fact]
    public async Task ListKeys_ReturnsCorrectCount_WithExpired()
    {
        // Arrange
        var key = $"{nameof(ListKeys_ReturnsCorrectCount_WithExpired)}";
        var sut = (T) Activator.CreateInstance(typeof(T), _options)!;
        await sut.Write($"{key}1", "value").ConfigureAwait(false);
        await sut.Write($"{key}2", "value").ConfigureAwait(false);
        await sut.Write($"{key}3", "value").ConfigureAwait(false);
        await sut.Write($"{key}4", "value", 61).ConfigureAwait(false);

        await Task.Delay(62000).ConfigureAwait(false);
        
        // Act
        var result = await sut.ListKeys(key).ConfigureAwait(false);
        
        // Assert
        result.Should().HaveCount(3);
    }
    
    [Fact]
    public async Task ListKeys_ReturnsCorrectCount()
    {
        // Arrange
        var key = $"{nameof(ListKeys_ReturnsCorrectCount)}";
        var sut = (T) Activator.CreateInstance(typeof(T), _options)!;
        await sut.Write($"{key}1", "value").ConfigureAwait(false);
        await sut.Write($"{key}2", "value").ConfigureAwait(false);
        
        // Act
        var result = await sut.ListKeys().ConfigureAwait(false);
        
        // Assert
        result.Should().HaveCountGreaterOrEqualTo(2); // since the storage is shared between all tests, there could be more than 2 entries
    }
}