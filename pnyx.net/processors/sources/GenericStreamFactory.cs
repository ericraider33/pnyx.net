using System;
using System.IO;
using pnyx.net.api;

namespace pnyx.net.processors.sources
{
    public class GenericStreamFactory : IStreamFactory, IDisposable
    {
        private Stream stream;

        public GenericStreamFactory(Stream stream)
        {
            this.stream = stream;
        }

        public Stream openStream()
        {
            return stream;
        }

        public void closeStream()
        {
            stream.Close();            
        }

        public void Dispose()
        {
            if (stream != null)
                stream.Dispose();

            stream = null;
        }
    }
}