namespace GerwimFeiken.Cache.Redis.Options;

public class RedisOptions : Cache.Options.Options, IRedisOptions
{
    public string? Configuration { get; set; }
}