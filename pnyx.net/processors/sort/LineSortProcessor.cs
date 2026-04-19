using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using pnyx.net.util;

namespace pnyx.net.processors.sort;

public class LineSortProcessor : ILineProcessor, ILinePart, IAsyncDisposable
{
    public ILineProcessor? processor { get; private set; }       
    public int bufferSize { get; }
    public String tempDirectory { get; }
    public IComparer<String> comparer { get; }
    private readonly PnyxSortedList<String> buffer;
    private readonly String tempFileKey;
    private int fileNumber;
    private readonly List<String> sortFiles = new List<String>();
    private readonly List<String> tempFiles = new List<String>();

    public LineSortProcessor
    (
        IComparer<String> comparer, 
        bool unique = false,
        String? tempDirectory = null,
        int bufferSize = 10000
    )
    {
        this.bufferSize = bufferSize;
        this.tempDirectory = tempDirectory ?? Directory.GetCurrentDirectory();
        this.comparer = comparer;
            
        buffer = new PnyxSortedList<String>(bufferSize, comparer, unique);            
        tempFileKey = ParseExtensions.extractAlphaNumeric(Guid.NewGuid().ToString());
    }

    public async Task processLine(String line)
    {
        buffer.add(line);

        if (buffer.count >= bufferSize)
            await emptyBuffer();
    }

    public async Task endOfFile()
    {
        if (buffer.count > 0)
            await emptyBuffer();

        await sort();

        if (sortFiles.Count > 0)
        {
            await using FileStream stream = new FileStream(sortFiles[0], FileMode.Open, FileAccess.Read);
            using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            
            String? line;
            while ((line = await reader.ReadLineAsync()) != null)
                await processor!.processLine(line);
        }                
            
        await processor!.endOfFile();
    }

    public void setNextLineProcessor(ILineProcessor next)
    {
        processor = next;
    }
    
    private async Task emptyBuffer()
    {
        String fileName = $"line_sort_{tempFileKey}_{++fileNumber}.txt";
        String filePath = Path.Combine(tempDirectory, fileName);
        tempFiles.Add(filePath);            // master list of files to be cleaned up
        sortFiles.Add(filePath);            // transient files used to output sorted 

        await using (FileStream stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
        {
            await using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
            {
                await buffer.visit(writer.WriteLineAsync);
            }
        }
            
        buffer.clear();
    }
    
    /// <summary>
    /// Uses sorted files to perform an infinitely scalable sort without requiring contents of files to be
    /// held in memory.
    ///
    /// Data is chunked into batches, which are written to files via the 'emptyBuffer' method. For there, pairs of
    /// the files are combined into a single sorted file. The process continues until only one sorted file remains.
    /// </summary>
    private async Task sort()
    {
        while (sortFiles.Count > 1)
        {
            for (int i = 0; i < sortFiles.Count-1; i++)
            {
                String filePathA = sortFiles[i];
                String filePathB = sortFiles[i + 1];

                // Combines files
                String filePathC = await sortPair(filePathA, filePathB);
                    
                // Replaces pair with resultant file
                sortFiles[i] = filePathC;
                sortFiles.RemoveAt(i+1);
            }
        }
    }

    private async Task<String> sortPair(String filePathA, String filePathB)
    {
        String fileNameC = $"line_sort_{tempFileKey}_{++fileNumber}.txt";
        String filePathC = Path.Combine(tempDirectory, fileNameC);
        tempFiles.Add(filePathC);            // master list of files to be cleaned up

        FileStream? streamA = null, streamB = null, streamC = null;
        StreamReader? readerA = null, readerB = null;
        StreamWriter? writer = null;

        try
        {
            streamA = new FileStream(filePathA, FileMode.Open, FileAccess.Read);
            streamB = new FileStream(filePathB, FileMode.Open, FileAccess.Read);
            streamC = new FileStream(filePathC, FileMode.CreateNew, FileAccess.Write);

            readerA = new StreamReader(streamA, Encoding.UTF8);
            readerB = new StreamReader(streamB, Encoding.UTF8);
            writer = new StreamWriter(streamC, Encoding.UTF8);

            String? lineA = null;
            String? lineB = null;
            bool endA = false, endB = false;
            while (!endA || !endB)
            {
                if (!endA && lineA == null)
                {
                    lineA = await readerA.ReadLineAsync();
                    endA = lineA == null;
                }

                if (!endB && lineB == null)
                {
                    lineB = await readerB.ReadLineAsync();
                    endB = lineB == null;
                }
                    
                if (lineA != null && lineB != null)
                {
                    if (comparer.Compare(lineA, lineB) <= 0)
                    {
                        await writer.WriteLineAsync(lineA);
                        lineA = null;
                    }
                    else
                    {
                        await writer.WriteLineAsync(lineB);
                        lineB = null;
                    }
                }
                else if (lineA != null)
                {
                    await writer.WriteLineAsync(lineA);
                    lineA = null;
                }
                else if (lineB != null)
                {
                    await writer.WriteLineAsync(lineB);
                    lineB = null;
                }
            }
                
            // Clears out any remaining lines
            if (lineA != null)
                await writer.WriteLineAsync(lineA);
            if (lineB != null)
                await writer.WriteLineAsync(lineB);                
        }
        finally
        {
            if (writer != null) await writer.FlushAsync();
            if (streamC != null) await streamC.FlushAsync();
            
            if (readerA != null) readerA.Dispose();
            if (readerB != null) readerB.Dispose();
            if (writer != null) await writer.DisposeAsync();
                
            if (streamA != null) await streamA.DisposeAsync();
            if (streamB != null) await streamB.DisposeAsync();
            if (streamC != null) await streamC.DisposeAsync();
        }

        return filePathC;
    }

    public ValueTask DisposeAsync()
    {
        foreach (String filePath in tempFiles)
        {
            FileInfo toRemove = new FileInfo(filePath);
            if (toRemove.Exists)
                toRemove.Delete();                
        }
            
        tempFiles.Clear();
        return ValueTask.CompletedTask;
    }
}