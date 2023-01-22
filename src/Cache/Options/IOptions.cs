namespace GerwimFeiken.Cache.Options;

public interface IOptions
{
    /// <summary>
    /// Defaults to 86400 (1 day).
    /// </summary>
    public int DefaultExpirationTtl { get; set; }
}