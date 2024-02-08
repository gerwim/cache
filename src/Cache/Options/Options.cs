using Newtonsoft.Json;

namespace GerwimFeiken.Cache.Options;

public abstract class Options : IOptions
{
    /// <summary>
    /// Defaults to 86400 (1 day).
    /// </summary>
    public int DefaultExpirationTtl { get; set; } = 86400;
    
    /// <summary>
    /// (De)serialization settings. If set, this will overwrite the default settings.
    /// </summary>
    public JsonSerializerSettings? JsonSerializerSettings { get; set; }
}