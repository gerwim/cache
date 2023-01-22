using System;
using System.Runtime.Serialization;

namespace GerwimFeiken.Cache.Exceptions;

[Serializable]
public class WriteException : Exception
{
    public WriteException()
    {
    }

    protected WriteException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public WriteException(string message) : base(message)
    {
    }

    public WriteException(string message, Exception innerException) : base(message, innerException)
    {
    }
}