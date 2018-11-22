using System;
using System.IO;
using pnyx.net.api;

namespace pnyx.net.processors.readers
{
    public class StringStreamFactory : IStreamFactory, IDisposable
    {
        public readonly String source;
        
        private MemoryStream stream;

        public StringStreamFactory(string source)
        {
            this.source = source;
        }

        public Stream openStream()
        {
            if (stream != null)
            {
                resetStream();
                return stream;
            }
            
            stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);

            writer.Write(source);
            writer.Flush();
            
            stream.Position = 0;
            return stream;
        }

        public void resetStream()
        {
            stream.Position = 0;                        
        }
        
        public void closeStream()
        {
            // no-op
        }

        public void Dispose()
        {
            if (stream != null)
                stream.Dispose();

            stream = null;
        }
    }
}