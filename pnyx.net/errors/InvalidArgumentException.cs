using System;
using System.Runtime.Serialization;

namespace pnyx.net.errors
{
    public class InvalidArgumentException : PnyxException
    {
        public InvalidArgumentException()
        {
        }

        public InvalidArgumentException(String message) : base(message)
        {
        }

        public InvalidArgumentException(String message, params object[] replacements) : base(message, replacements)
        {
        }

        public InvalidArgumentException(String message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}