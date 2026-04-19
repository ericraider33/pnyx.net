using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using pnyx.net.api;
using pnyx.net.errors;
using pnyx.net.util;

namespace pnyx.net.processors.sources;

public class TailStreamFactory : IStreamFactory, IAsyncDisposable
{
    public StreamInformation streamInformation { get; }        
    public int limit { get; }
    
    private IStreamFactory? source;
    private long position;
    private Stream? stream;
    private Encoding? encoding;

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

        if (source == null)
            throw new IllegalStateException("Source has been disposed");
        
        stream = source.openStream();
        if (!stream.CanSeek)
            throw new IllegalStateException("Use TailBuffer for streams that do not support seeking");

        readEncoding();
        findPosition();
            
        return stream;
    }

    public void closeStream()
    {
        if (stream == null)
            return;
        
        stream.Close();
    }
    
    public async ValueTask DisposeAsync()
    {
        if (source != null && source is IAsyncDisposable)
            await ((IAsyncDisposable)source).DisposeAsync();

        stream = null;
    }

    private void readEncoding()
    {
        StreamReader reader = new StreamReader(stream!, streamInformation.defaultEncoding, true);
        reader.Read();        // ignores results, simply primes the encoding via BOM characters
        encoding = reader.CurrentEncoding;
            
        // Sets encoding 
        streamInformation.updateStreamEncoding(encoding);
    }

    private void findPosition()
    {
        if (encoding!.IsSingleByte)
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
        stream!.Seek(0, SeekOrigin.End);
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