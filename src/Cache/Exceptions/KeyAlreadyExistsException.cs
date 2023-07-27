using System;
using System.Runtime.Serialization;

namespace GerwimFeiken.Cache.Exceptions;

[Serializable]
public class KeyAlreadyExistsException : Exception
{
    public KeyAlreadyExistsException()
    {
    }

    protected KeyAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public KeyAlreadyExistsException(string message) : base(message)
    {
    }

    public KeyAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
    {
    }
}