namespace GerwimFeiken.Cache.Redis.Options;

public class RedisOptions : Cache.Options.Options, IRedisOptions
{
    public string? Configuration { get; set; }
    /// <summary>
    /// If set to true, upon timeouts the cache will return default values and drop all writes
    /// </summary>
    public bool IgnoreTimeouts { get; set; }
}