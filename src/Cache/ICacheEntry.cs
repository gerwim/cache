using GerwimFeiken.Cache.Models;

namespace GerwimFeiken.Cache;

internal interface ICacheEntry
{
    T GetValue<T>();
    void SetValue<T>(T value);
    CacheEntryMetadata? Metadata { get; }
}