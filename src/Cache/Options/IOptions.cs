using MessagePack;

namespace GerwimFeiken.Cache.Options;

public interface IOptions
{
    /// <summary>
    /// Defaults to 86400 (1 day).
    /// </summary>
    public int DefaultExpirationTtl { get; set; }

    /// <summary>
    /// (De)serialization settings. If set, this will overwrite the default settings.
    /// </summary>
    MessagePackSerializerOptions? SerializerOptions { get; set; }
}