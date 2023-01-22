namespace GerwimFeiken.Cache.Options;

public abstract class Options : IOptions
{
    /// <summary>
    /// Defaults to 86400 (1 day).
    /// </summary>
    public int DefaultExpirationTtl { get; set; } = 86400;
}