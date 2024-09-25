using DotNet.Testcontainers.Containers;
using Testcontainers.Redis;

namespace Cache.Tests.Integration.Fixtures;

public class RedisFixture : IDisposable
{
    public RedisFixture()
    {
        RedisContainer = new RedisBuilder()
            .WithImage("redis:6.2")
            .WithPortBinding("6379")
            .Build();
        RedisContainer.StartAsync().GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        RedisContainer.DisposeAsync();
    }

    public IContainer RedisContainer { get; private set; }
}