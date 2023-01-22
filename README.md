# cache
.NET library which provides key value cache functionality

# Supported backends
* InMemoryCache
* Cloudflare KV storage

## General configuration
You can configure the default expiration TTL by setting the environment variable `GerwimFeiken.Cache__ExpirationTtl`, or through your `appsettings.json`:

```
...
"GerwimFeiken.Cache": {
  "DefaultExpirationTtl": 3600
}
...
```
## InMemoryCache
No additional configuration is needed.

## Cloudflare KV storage
You'll need to configure these two environment variables (or configure them in `appsettings.json`):
* GerwimFeiken.Cache__Cloudflare__ApiToken
* GerwimFeiken.Cache__Cloudflare__KVUrl

or configured in `appsettings.json`:
```
...
"GerwimFeiken.Cache": {
  "Cloudflare": {
    "ApiToken": "xxxx",
    "KVUrl": "https://api.cloudflare.com/..."
  }
}
...
```

### API token
You can create the ApiToken at the [Cloudflare profile page](https://dash.cloudflare.com/profile/api-tokens). 

### KV url 
The KV URL consists of your account ID and your namespace ID. This is a correct format:
`https://api.cloudflare.com/client/v4/accounts/XXXXXXXX/storage/kv/namespaces/YYYYYYYY`

When visiting the KV dashboard page, you can extract both ID's from the URL:
`https://dash.cloudflare.com/XXXXXXXX/workers/kv/namespaces/YYYYYYYY`



# How to use
Register your cache backend:

```
using GerwimFeiken.Cache;
...
services.AddSingleton<ICache, InMemoryCache>();
```

# Development
To pack this library to use on NuGet, run the `dotnet pack` command:
`dotnet pack -c release`