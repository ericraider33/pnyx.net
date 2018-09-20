using System;
using System.Runtime.Serialization;

namespace pnyx.net.errors
{
    public class IllegalStateException : PnyxException
    {
        public IllegalStateException()
        {
        }

        public IllegalStateException(string message) : base(message)
        {
        }

        public IllegalStateException(string message, params object[] replacements) : base(message, replacements)
        {
        }

        public IllegalStateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IllegalStateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}