using System;
using System.Runtime.Serialization;

namespace pnyx.net.errors
{
    public class InvalidArgumentException : PnyxException
    {
        public InvalidArgumentException()
        {
        }

        public InvalidArgumentException(string message) : base(message)
        {
        }

        public InvalidArgumentException(string message, params object[] replacements) : base(message, replacements)
        {
        }

        public InvalidArgumentException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}