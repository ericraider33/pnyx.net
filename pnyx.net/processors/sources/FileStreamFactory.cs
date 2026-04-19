using System;
using System.IO;
using System.Threading.Tasks;
using pnyx.net.api;
using pnyx.net.errors;

namespace pnyx.net.processors.sources;

public class FileStreamFactory : IStreamFactory, IAsyncDisposable
{
    public String path { get; }
    public FileMode mode { get; }
    public FileAccess access { get; }

    private FileStream? fileStream;

    public FileStreamFactory(string path, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read)
    {
        this.mode = mode;
        this.access = access;
        this.path = path;
    }

    public Stream openStream()
    {
        if (fileStream != null)
        {
            resetStream();
            return fileStream;
        }
            
        fileStream = new FileStream(path, mode, access);
        return fileStream;
    }

    private void resetStream()
    {
        if (fileStream == null)
            throw new IllegalStateException("You must open the stream before resetting it");
        
        fileStream.Seek(0, SeekOrigin.Begin);
    }

    public void closeStream()
    {
        if (fileStream == null)
            throw new IllegalStateException("You must open the stream before closing it");
        
        fileStream.Close();
    }

    public async ValueTask DisposeAsync()
    {
        if (fileStream != null)
            await fileStream.DisposeAsync();

        fileStream = null;
    }
}