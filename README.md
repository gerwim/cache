# GerwimFeiken.Cache
.NET library which provides key value cache functionality.

# Supported providers
* In memory cache (`InMemoryCache`)
* Cloudflare KV storage (`CloudflareCache`)

# How to use
## Dependency injection

```
builder.Services.AddSingleton<ICache>(new Provider());
```
where `Provider` is one of the supported providers.

## Console application / class library
```
ICache cache = new Provider()
```
## General configuration
When creating the provider, you'll need to pass options. These will extend from [options](src/Cache/Options/Options.cs).
You can override these values when instantiating your provider.

### InMemoryCache
Requires the package `GerwimFeiken.Cache.InMemory`.

### CloudflareCache
Requires the package `GerwimFeiken.Cache.Cloudflare`.

See [documentation](src/Cache.Cloudflare/README.md) for additional requirements.

# Development
## Tests
Some of the tests might be an integration test.
You'll need to specify correct options for these to work.
## Pack
To pack this library to use on NuGet, run the `dotnet pack` command:
`dotnet pack -c release`