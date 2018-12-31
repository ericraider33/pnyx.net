using System;
using System.Runtime.Serialization;

namespace pnyx.net.errors
{
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

        protected PnyxException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}