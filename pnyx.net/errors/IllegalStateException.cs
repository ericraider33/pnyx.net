using System;

namespace pnyx.net.errors;

public class IllegalStateException : PnyxException
{
    public IllegalStateException()
    {
    }

    public IllegalStateException(String message) : base(message)
    {
    }

    public IllegalStateException(String message, params Object[] replacements) : base(message, replacements)
    {
    }

    public IllegalStateException(String message, Exception innerException) : base(message, innerException)
    {
    }
}