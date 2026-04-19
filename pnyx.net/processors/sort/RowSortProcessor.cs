using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using pnyx.net.impl.csv;
using pnyx.net.util;

namespace pnyx.net.processors.sort;

public class RowSortProcessor : IRowProcessor, IRowPart, IAsyncDisposable
{
    public IRowProcessor? processor { get; private set; }       
    public int bufferSize { get; }
    public String tempDirectory { get; }
    public IComparer<List<String?>> comparer { get; }
    
    private readonly PnyxSortedList<List<String?>> buffer;
    private readonly String tempFileKey;
    private int fileNumber;
    private readonly List<String> sortFiles = new List<String>();
    private readonly List<String> tempFiles = new List<String>();

    public RowSortProcessor
    (
        IComparer<List<String?>> comparer, 
        bool unique = false,
        String? tempDirectory = null,
        int bufferSize = 10000
    )
    {
        this.bufferSize = bufferSize;
        this.tempDirectory = tempDirectory ?? Directory.GetCurrentDirectory();
        this.comparer = comparer;
            
        buffer = new PnyxSortedList<List<String?>>(bufferSize, comparer, unique);            
        tempFileKey = ParseExtensions.extractAlphaNumeric(Guid.NewGuid().ToString());
    }

    public async Task rowHeader(List<String> header)
    {
        await processor!.rowHeader(header);
    }

    public async Task processRow(List<String?> row)
    {
        buffer.add(row);

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
            await using CsvReader reader = new CsvReader(stream, Encoding.UTF8);

            List<String?>? row;
            while ((row = await reader.readRow()) != null)
                await processor!.processRow(row);
        }                
            
        await processor!.endOfFile();
    }

    public void setNextRowProcessor(IRowProcessor next)
    {
        processor = next;
    }

    private async Task emptyBuffer()
    {
        String fileName = $"row_sort_{tempFileKey}_{++fileNumber}.txt";
        String filePath = Path.Combine(tempDirectory, fileName);
        tempFiles.Add(filePath);            // master list of files to be cleaned up
        sortFiles.Add(filePath);            // transient files used to output sorted 

        await using (FileStream stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
        {
            await using (CsvWriter writer = new CsvWriter(stream, Encoding.UTF8))
            {
                await buffer.visit(writer.writeRow);
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
        String fileNameC = $"row_sort_{tempFileKey}_{++fileNumber}.txt";
        String filePathC = Path.Combine(tempDirectory, fileNameC);
        tempFiles.Add(filePathC);            // master list of files to be cleaned up

        FileStream? streamA = null, streamB = null, streamC = null;
        CsvReader? readerA = null, readerB = null;
        CsvWriter? writer = null;

        try
        {
            streamA = new FileStream(filePathA, FileMode.Open, FileAccess.Read);
            streamB = new FileStream(filePathB, FileMode.Open, FileAccess.Read);
            streamC = new FileStream(filePathC, FileMode.CreateNew, FileAccess.Write);

            readerA = new CsvReader(streamA, Encoding.UTF8);
            readerB = new CsvReader(streamB, Encoding.UTF8);
            writer = new CsvWriter(streamC, Encoding.UTF8);

            List<String?>? rowA = null;
            List<String?>? rowB = null;
            while (!readerA.endOfFile || !readerB.endOfFile)
            {
                if (!readerA.endOfFile && rowA == null)
                    rowA = await readerA.readRow();

                if (!readerB.endOfFile && rowB == null)
                    rowB = await readerB.readRow();
                    
                if (rowA != null && rowB != null)
                {
                    if (comparer.Compare(rowA, rowB) <= 0)
                    {
                        await writer.writeRow(rowA);
                        rowA = null;
                    }
                    else
                    {
                        await writer.writeRow(rowB);
                        rowB = null;
                    }
                }
                else if (rowA != null)
                {
                    await writer.writeRow(rowA);
                    rowA = null;
                }
                else if (rowB != null)
                {
                    await writer.writeRow(rowB);
                    rowB = null;
                }
            }
                
            // Clears out any remaining lines
            if (rowA != null)
                await writer.writeRow(rowA);
            if (rowB != null)
                await writer.writeRow(rowB);                
        }
        finally
        {
            if (readerA != null) await readerA.DisposeAsync();
            if (readerB != null) await readerB.DisposeAsync();
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