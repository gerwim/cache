using System;
using System.Runtime.Serialization;

namespace GerwimFeiken.Cache.Exceptions;

[Serializable]
public class ReadException : Exception
{
    public ReadException()
    {
    }

    protected ReadException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ReadException(string message) : base(message)
    {
    }

    public ReadException(string message, Exception innerException) : base(message, innerException)
    {
    }
}