namespace GerwimFeiken.Cache.Models;

internal sealed class CacheEntry<T> : ICacheEntry
{
    public CacheEntry(T value, CacheEntryMetadata? metadata)
    {
        Value = value;
        Metadata = metadata;
    }

    private T Value { get; set; }

    public T1 GetValue<T1>()
    {
        return (T1)(object)Value!;
    }

    public void SetValue<T1>(T1 value)
    {
        Value = (T)(object)value!;
    }

    public CacheEntryMetadata? Metadata { get; }
}