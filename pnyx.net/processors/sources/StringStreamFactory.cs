using System;
using System.IO;
using System.Threading.Tasks;
using pnyx.net.api;

namespace pnyx.net.processors.sources;

public class StringStreamFactory : IStreamFactory, IAsyncDisposable
{
    public String source { get; }
        
    private MemoryStream? stream;

    public StringStreamFactory(String source)
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
        if (stream == null)
            return;
        
        stream.Position = 0;                        
    }
        
    public void closeStream()
    {
        // no-op
    }

    public async ValueTask DisposeAsync()
    {
        if (stream != null)
            await stream.DisposeAsync();

        stream = null;
    }
}