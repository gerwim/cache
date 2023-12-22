using System;
using System.Runtime.Serialization;

namespace GerwimFeiken.Cache.Exceptions;

[Serializable]
public class InvalidTypeException : Exception
{
    public InvalidTypeException()
    {
    }

    protected InvalidTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public InvalidTypeException(string message) : base(message)
    {
    }

    public InvalidTypeException(string message, Exception innerException) : base(message, innerException)
    {
    }
}