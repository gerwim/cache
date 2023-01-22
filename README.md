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
You can override these values when instantiating your provider, e.g.

```
ICache cache = new InMemoryCache(new InMemoryOptions
{
    DefaultExpirationTtl = 3600
});
```
### InMemoryCache
No additional configuration is needed.

### CloudflareCache
```
ICache cache = new CloudflareCache(new CloudflareOptions
{
    DefaultExpirationTtl = 86400, // can be omitted
    ApiToken = "XXX",
    AccountId = "YYYY",
    NamespaceId = "ZZZZ"
});
```

**Api Token**

You can create the ApiToken at the [Cloudflare profile page](https://dash.cloudflare.com/profile/api-tokens). 

**Account and namespace ID**

When visiting the KV dashboard page, you can extract both ID's from the URL:
`https://dash.cloudflare.com/YYYY/workers/kv/namespaces/ZZZZ`

Where `YYYY` is your `AccountId` and `ZZZZ` is your `NamespaceId`.


# Development
## Tests
Some of the tests might be an integration test.
You'll need to specify correct options for these to work.
## Pack
To pack this library to use on NuGet, run the `dotnet pack` command:
`dotnet pack -c release`