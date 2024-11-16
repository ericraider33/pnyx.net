using System;

namespace pnyx.net.errors;

public class PnyxException : Exception
{
    public PnyxException()
    {
    }

    public PnyxException(String message) : base(message)
    {
    }

    public PnyxException(String message, params Object[] replacements) : base(String.Format(message, replacements))
    {
    }

    public PnyxException(String message, Exception innerException) : base(message, innerException)
    {
    }
}