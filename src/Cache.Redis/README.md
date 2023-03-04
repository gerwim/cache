# GerwimFeiken.Cache.Redis

```
ICache cache = new RedisCache(new RedisOptions
{
    DefaultExpirationTtl = 86400, // can be omitted
    Configuration = "127.0.0.1"
});
```

**Configuration**

This is the Redis configuration string setting. Please see the  [official documentation](https://stackexchange.github.io/StackExchange.Redis/Configuration) for all values.