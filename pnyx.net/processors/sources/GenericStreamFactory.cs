using System;
using System.IO;
using System.Threading.Tasks;
using pnyx.net.api;

namespace pnyx.net.processors.sources;

public class GenericStreamFactory : IStreamFactory, IAsyncDisposable
{
    private Stream? stream;

    public GenericStreamFactory(Stream stream)
    {
        this.stream = stream;
    }

    public Stream openStream()
    {
        if (stream == null)
            throw new InvalidOperationException("Stream has been disposed");
        
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
        if (stream != null)
            await stream.DisposeAsync();

        stream = null;
    }
}