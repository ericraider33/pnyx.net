using System;
using System.IO;
using System.Threading.Tasks;
using pnyx.net.util;

namespace pnyx.net.processors.dest;

// p.writeSplit("icd10.$0.txt", 99, "c:/dev/pnyx.net/pnyx.net.test/files/tab");
public class LineProcessorSplit : ILineProcessor, IAsyncDisposable
{
    public readonly String? path;
    public readonly int limit;
    public readonly String fileNamePattern;
    public readonly StreamInformation streamInformation;
    public TextWriter? writer { get; private set; }

    private String? previousLine;
    private int lineNumber;
    private int fileNumber;        
        
    public LineProcessorSplit(StreamInformation streamInformation, String fileNamePattern, int limit = 100, String? path = null)
    {
        this.streamInformation = streamInformation;
        this.fileNamePattern = fileNamePattern;
        this.limit = limit;
        this.path = path;
    }

    public async Task processLine(String line)
    {
        if (previousLine != null && writer != null)
        {
            lineNumber++;
            if (lineNumber > limit)
            {
                await nextFile();
                lineNumber = 1;
            }
                
            await writer.WriteAsync(previousLine);
            await writer.WriteAsync(streamInformation.getOutputNewline());
        }
        else
        {
            await nextFile();
        }

        previousLine = line;
    }

    private async Task nextFile()
    {
        if (writer != null)
        {
            await writer.FlushAsync();
            writer.Close();
            await writer.DisposeAsync();
        }
            
        fileNumber++;

        String fileName = fileNamePattern.Replace("$0", $"{fileNumber:000}");

        String combined = fileName;
        if (path != null)
            combined = Path.Combine(path, fileName);
                
        FileStream fs = new FileStream(combined, FileMode.Create, FileAccess.Write);
        writer = new StreamWriter(fs, streamInformation.getOutputEncoding());            
    }

    public async Task endOfFile()
    {
        if (previousLine != null && writer != null)
        {              
            lineNumber++;
            if (lineNumber > limit)
            {
                await nextFile();
                lineNumber = 1;
            }
                
            await writer.WriteAsync(previousLine);
            if (streamInformation.endsWithNewLine)
                await writer.WriteAsync(streamInformation.getOutputNewline());
        }

        previousLine = null;
            
        if (writer != null)
            await writer.FlushAsync();
    }

    public void Dispose()
    {
    }

    public ValueTask DisposeAsync()
    { 
        if (writer != null)
        {
            writer.Flush();
            writer.Dispose();
        }

        writer = null;
        return ValueTask.CompletedTask;
    }
}