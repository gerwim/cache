# GerwimFeiken.Cache.Redis

```
ICache cache = new RedisCache(new RedisOptions
{
    DefaultExpirationTtl = 86400, // can be omitted
    Configuration = "127.0.0.1",
    IgnoreTimeouts = false,
});
```

**Configuration**

This is the Redis configuration string setting. Please see the  [official documentation](https://stackexchange.github.io/StackExchange.Redis/Configuration) for all values.

**IgnoreTimeouts**

If this value is set to true, the cache will ignore timeout related isues and simply act as if the cache was not present at all.
Depending on your connection string settings, cache lookups will still take some time due to timeout settings.

For instance, setting the connection string to `{{HOST}},abortConnect=false,connectTimeout=1000,connectRetry=0` will allow for relative fast failures. Please see the  [official documentation](https://stackexchange.github.io/StackExchange.Redis/Configuration) for all values.