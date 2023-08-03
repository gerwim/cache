namespace GerwimFeiken.Cache.Models;

public enum ReadReason
{
    Unknown,
    KeyDoesNotExist,
    Timeout,
}

public enum WriteReason
{
    Unknown,
    Timeout,
}