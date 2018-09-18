using System;
using System.Runtime.Serialization;

namespace pnyx.net.errors
{
    public class PnyxException : Exception
    {
        public PnyxException()
        {
        }

        public PnyxException(string message) : base(message)
        {
        }

        public PnyxException(string message, object[] replacements) : base(String.Format(message, replacements))
        {
        }

        public PnyxException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PnyxException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}