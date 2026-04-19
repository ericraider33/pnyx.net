using System;
using System.IO;
using System.Threading.Tasks;
using pnyx.net.errors;
using pnyx.net.util;

namespace pnyx.net.processors.dest;

public class LineProcessorToStream : ILineProcessor, IAsyncDisposable
{
    public readonly StreamInformation streamInformation;

    private TextWriter? writer;
    private Stream? stream;
    private String? previousLine;
        
    public LineProcessorToStream(StreamInformation streamInformation, Stream stream)
    {
        this.stream = stream;
        this.streamInformation = streamInformation;
    }

    public async Task processLine(String line)
    {
        if (previousLine != null && writer != null)
        {
            await writer.WriteAsync(previousLine);
            await writer.WriteAsync(streamInformation.getOutputNewline());
        }
        else
        {
            if (stream == null)
                throw new IllegalStateException("Stream has already been disposed");
            
            writer = new StreamWriter(stream, streamInformation.getOutputEncoding());
        }

        previousLine = line;
    }

    public async Task endOfFile()
    {
        if (previousLine != null && writer != null)
        {              
            await writer.WriteAsync(previousLine);
            if (streamInformation.endsWithNewLine)
                await writer.WriteAsync(streamInformation.getOutputNewline());
        }
        else
        {
            if (stream == null)
                throw new IllegalStateException("Stream has already been disposed");

            writer = new StreamWriter(stream, streamInformation.getOutputEncoding());                        
        }

        previousLine = null;
        await writer.FlushAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (writer != null)
        {
            await writer.FlushAsync();
            await writer.DisposeAsync();
        }

        stream = null;
        writer = null;
        previousLine = null;
    }
}