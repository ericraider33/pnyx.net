using System;

namespace pnyx.net.errors;

public class InvalidArgumentException : PnyxException
{
    public InvalidArgumentException()
    {
    }

    public InvalidArgumentException(String message) : base(message)
    {
    }

    public InvalidArgumentException(String message, params Object[] replacements) : base(message, replacements)
    {
    }

    public InvalidArgumentException(String message, Exception innerException) : base(message, innerException)
    {
    }
}