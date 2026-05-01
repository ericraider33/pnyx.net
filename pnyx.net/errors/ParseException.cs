using System;

namespace pnyx.net.errors;

public class ParseException : PnyxException
{
    public ParseException()
    {
    }

    public ParseException(string message) : base(message)
    {
    }

    public ParseException(string message, Exception innerException) : base(message, innerException)
    {
    }
}