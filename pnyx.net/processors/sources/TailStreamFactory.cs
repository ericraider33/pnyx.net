using System;
using System.IO;
using System.Text;
using pnyx.net.api;
using pnyx.net.errors;
using pnyx.net.util;

namespace pnyx.net.processors.sources
{
    public class TailStreamFactory : IStreamFactory, IDisposable
    {
        public int limit;
        private IStreamFactory source;
        private long position;
        private Stream stream;
        private Encoding encoding;
        private readonly StreamInformation streamInformation;        

        public TailStreamFactory(StreamInformation streamInformation, IStreamFactory source, int limit)
        {
            this.streamInformation = streamInformation;
            this.limit = limit;
            this.source = source;
        }

        public Stream openStream()
        {
            if (stream != null)
            {
                stream.Seek(position, SeekOrigin.Begin);
                return stream;
            }

            stream = source.openStream();
            if (!stream.CanSeek)
                throw new IllegalStateException("Use TailBuffer for streams that do not support seeking");

            readEncoding();
            findPosition();
            
            return stream;
        }

        public void closeStream()
        {
            source.closeStream();
        }

        public void Dispose()
        {
            if (source != null && source is IDisposable)
               ((IDisposable)source).Dispose();
            source = null;
        }

        private void readEncoding()
        {
            StreamReader reader = new StreamReader(stream, streamInformation.defaultEncoding, true);
            reader.Read();        // ignores results, simply primes the encoding via BOM characters
            encoding = reader.CurrentEncoding;
            
            //
            // Sets encoding and turns off BOM detection since Processor will get stream in a mid-file position
            //
            streamInformation.defaultEncoding = encoding;
            streamInformation.detectEncodingFromByteOrderMarks = false;                     
        }

        private void findPosition()
        {
            if (encoding.IsSingleByte)
            {
                findPositionSingleByte();
            }
            else if (Equals(encoding, Encoding.UTF8))
            {
                //
                // NOTE: UTF-8 is variable byte, always sets first bit for multi-byte characters.
                // Therefore, '\n' only ever matches a newline, and can safely use             
                //
                findPositionSingleByte();
            }
            else            
                throw new InvalidArgumentException("TailStreamFactory does not support encoding {0}", encoding.EncodingName);                        
        }
        
        private void findPositionSingleByte()
        {
            int count = 0;
            stream.Seek(0, SeekOrigin.End);
            position = stream.Length - 1;

            stream.ReadByte();        // ignores last value            
            
            while (position > 0 && count < limit)
            {
                stream.Seek(-2, SeekOrigin.Current);               
                int current = stream.ReadByte();
                if (current == '\n')
                    count++;
            }
        }        
    }
}