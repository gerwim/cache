using System;
using System.Runtime.Serialization;

namespace GerwimFeiken.Cache.Exceptions;

[Serializable]
public class DeleteException : Exception
{
    public DeleteException()
    {
    }

    protected DeleteException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public DeleteException(string message) : base(message)
    {
    }

    public DeleteException(string message, Exception innerException) : base(message, innerException)
    {
    }
}