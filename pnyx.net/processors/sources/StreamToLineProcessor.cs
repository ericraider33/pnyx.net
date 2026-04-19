using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using pnyx.net.api;
using pnyx.net.errors;
using pnyx.net.util;

namespace pnyx.net.processors.sources;

public class StreamToLineProcessor : IProcessor, ILinePart, ILineSource, IAsyncDisposable
{
    public IStreamFactory? streamFactory { get; private set; }        
    public StreamReader? reader { get; protected set; }
    public StreamInformation streamInformation { get; }
    public ILineProcessor? processor { get; protected set; }
        
    private readonly StringBuilder stringBuilder = new StringBuilder();
    private bool endOfFile;

    private char[]? buffer;
    private int? bufferIndex;
    private int? bufferCount;

    public StreamToLineProcessor(StreamInformation streamInformation, IStreamFactory streamFactory)
    {
        this.streamInformation = streamInformation;
        this.streamFactory = streamFactory;
    }
                
    public StreamToLineProcessor(StreamInformation streamInformation, Stream stream)
    {
        this.streamInformation = streamInformation;
        streamFactory = new GenericStreamFactory(stream);
    }                

    public virtual async Task process()
    {
        if (streamFactory == null)
            throw new IllegalStateException("StreamFactory has been disposed");
        
        Stream stream = streamFactory.openStream();
        reader = new StreamReader(stream, streamInformation.defaultEncoding, streamInformation.detectEncodingFromByteOrderMarks);
            
        endOfFile = false;
        String? line;
        while ((line = await readLine(streamInformation.lineNumber)) != null && streamInformation.active)
        {
            streamInformation.lineNumber++;
            await processor!.processLine(line);
        }

        if (!streamInformation.active)
            streamInformation.endsWithNewLine = line != null;

        await processor!.endOfFile();
        streamFactory.closeStream();            
    }
        
    protected virtual async Task<String?> readLine(int lineNumber)
    {
        stringBuilder.Clear();
        if (endOfFile)
            return null;            
            
        int num;
        while ((num = await readCharacter()) != -1)
        {
            switch (num)
            {
                case 10:
                    updateStreamInformation(lineNumber, "\n");                            
                    return stringBuilder.ToString();

                case 13:
                    if (await peekCharacter() == 10)
                    {
                        await readCharacter();            // consumes both \r\n
                        updateStreamInformation(lineNumber, "\r\n");                            
                    }
                    else
                        updateStreamInformation(lineNumber, "\r");
                        
                    return stringBuilder.ToString();
                        
                default:
                    stringBuilder.Append((char)num);
                    continue;
            }
        }            

        endOfFile = true;
        updateStreamInformation(lineNumber, null);

        if (stringBuilder.Length > 0)
            return stringBuilder.ToString();

        // Sets flag because an empty String
        if (lineNumber > 0)
            streamInformation.endsWithNewLine = true;
            
        return null;
    }

    private void updateStreamInformation(int lineNumber, String? newLine)
    {
        if (lineNumber > 0)
            return;

        streamInformation.updateStreamNewLine(newLine);
        streamInformation.updateStreamEncoding(reader!.CurrentEncoding);
    }

    public void setNextLineProcessor(ILineProcessor next)
    {
        processor = next;
    }

    private async Task<int> readCharacter()
    {
        if (bufferIndex.HasValue && bufferCount.HasValue && bufferIndex.Value < bufferCount.Value)
        {
            int index = bufferIndex.Value;
            bufferIndex = index + 1;
            return buffer![index];
        }

        // Checks if source is exhausted
        if (bufferCount == 0)
            return -1;

        // Reads next block of data
        buffer ??= new char[8096];
        bufferCount = await reader!.ReadAsync(buffer, 0, buffer.Length);
        if (bufferCount == 0)
        {
            bufferIndex = 0;            
            return -1;
        }
        
        bufferIndex = 1;
        return buffer[0];
    }

    private async Task<int> peekCharacter()
    {
        if (bufferIndex.HasValue && bufferCount.HasValue && bufferIndex.Value < bufferCount.Value)
            return buffer![bufferIndex.Value];

        // Checks if source is exhausted
        if (bufferCount == 0)
            return -1;

        // Reads next block of data
        buffer ??= new char[8096];
        bufferCount = await reader!.ReadAsync(buffer, 0, buffer.Length);
        if (bufferCount == 0)
        {
            bufferIndex = 0;            
            return -1;
        }
        
        bufferIndex = 0;
        return buffer[0];
    }

    public async ValueTask DisposeAsync()
    {
        if (reader != null)                    
            reader.Dispose();            
        reader = null;
            
        if (streamFactory is IAsyncDisposable disposable)
            await disposable.DisposeAsync();
        streamFactory = null;

        buffer = null;
        bufferIndex = null;
        bufferCount = null;
    }
}