# GerwimFeiken.Cache.Cloudflare

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