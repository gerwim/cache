using GerwimFeiken.Cache.Options;

namespace GerwimFeiken.Cache.Redis.Options;

public interface IRedisOptions : IOptions
{
    /// <summary>
    /// Redis configuration. Can be an endpoint or multiple.
    /// See the <a href="https://stackexchange.github.io/StackExchange.Redis/Configuration">official documentation</a>.
    /// </summary>
    public string? Configuration { get; set; }

    /// <summary>
    /// If set to true, upon timeouts the cache will return default values and drop all writes
    /// </summary>
    bool IgnoreTimeouts { get; set; }
}