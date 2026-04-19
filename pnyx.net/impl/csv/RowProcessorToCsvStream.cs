using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using pnyx.net.errors;
using pnyx.net.processors;
using pnyx.net.util;

namespace pnyx.net.impl.csv;

public class RowToCsvStream : IRowProcessor, IAsyncDisposable
{
    public Stream? stream { get; private set; }
    public TextWriter? writer { get; private set; }
    public StreamInformation streamInformation { get; }
    public CsvSettings settings { get; }

    private List<String?>? previousRow;
                
    public RowToCsvStream
    (
        StreamInformation streamInformation, 
        Stream stream,
        CsvSettings settings
    )
    {
        this.stream = stream;
        this.streamInformation = streamInformation;
        this.settings = settings;
    }

    public async Task rowHeader(List<String> header)
    {
        await processRow(header.toRow());
    }

    public async Task processRow(List<String?> row)
    {
        if (previousRow != null && writer != null)
        {
            await CsvUtil.writeRowAsync(writer, previousRow, settings.delimiter, settings.escapeChar, settings.charsNeedEscape);
            await writer.WriteAsync(streamInformation.getOutputNewline());
        }
        else
        {
            if (stream == null)
                throw new IllegalStateException($"Stream has already been disposed");
            
            writer = new StreamWriter(stream, streamInformation.getOutputEncoding());
        }

        previousRow = row;
    }

    public async Task endOfFile()
    {
        if (previousRow != null && writer != null)
        {              
            await CsvUtil.writeRowAsync(writer, previousRow, settings.delimiter, settings.escapeChar, settings.charsNeedEscape);
            if (streamInformation.endsWithNewLine)
                await writer.WriteAsync(streamInformation.getOutputNewline());
        }
        else
        {
            if (stream == null)
                throw new IllegalStateException($"Stream has already been disposed");
            
            writer = new StreamWriter(stream, streamInformation.getOutputEncoding());                        
        }

        previousRow = null;
        await writer.FlushAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (writer != null)
        {
            await writer.FlushAsync();
            await writer.DisposeAsync();
        }
        writer = null;

        if (stream != null)
            await stream.DisposeAsync();
        stream = null;
        previousRow = null;
    }
}