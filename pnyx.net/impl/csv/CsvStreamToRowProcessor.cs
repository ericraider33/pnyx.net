using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using pnyx.net.api;
using pnyx.net.errors;
using pnyx.net.processors;
using pnyx.net.util;

namespace pnyx.net.impl.csv;

public class CsvStreamToRowProcessor : IRowSource, IAsyncDisposable
{
    public CsvRowConverter rowConverter { get; }
    public CsvSettings settings  { get; }
    public bool hasHeader { get; set; }
    public StreamReader? reader { get; protected set; }
    public IRowProcessor? rowProcessor { get; private set; }
    public StreamInformation? streamInformation { get; protected set; }        
    public IStreamFactory? streamFactory { get; protected set; }
        
    private readonly StringBuilder stringBuilder = new StringBuilder();
    
    /// <summary>
    /// Returns true when the end of the file has been reached, either via `CsvReader` or via `CsvStreamToRowProcessor.process` method.
    /// Manually calling `readRow` will result a null being returned when `endOfFile` is true.
    /// </summary>
    public bool endOfFile { get; private set; }
        
    private readonly char[] buffer = new char[8096];
    private int bufferSize;
    private int bufferPosition;

    public CsvStreamToRowProcessor(CsvSettings? settings = null)
    {
        this.settings = settings ?? new CsvSettings();
        rowConverter = new CsvRowConverter(this.settings);
    }

    public IRowConverter getRowConverter()
    {
        return rowConverter;
    }

    public virtual async Task process()
    {
        if (streamFactory == null)
            throw new IllegalStateException("StreamFactory not initialized");
        if (streamInformation == null)
            throw new IllegalStateException("StreamInformation not initialized");
        if (rowProcessor == null)
            throw new IllegalStateException("RowProcessor not initialized");
        
        Stream stream = streamFactory.openStream();
        reader = new StreamReader(stream, streamInformation.defaultEncoding, streamInformation.detectEncodingFromByteOrderMarks);
            
        endOfFile = false;
        List<String?>? current;
        while ((current = await readRow(streamInformation.lineNumber)) != null && streamInformation.active)
        {
            streamInformation.lineNumber++;
            if (streamInformation.lineNumber == 1 && hasHeader)
                await rowProcessor.rowHeader(current.toHeader());
            else
                await rowProcessor.processRow(current);
        }

        if (!streamInformation.active)
            streamInformation.endsWithNewLine = current != null;

        await rowProcessor.endOfFile();
        streamFactory.closeStream();
    }
                
    private enum CsvState { StartOfLine, Quoted, Data, Seeking }

    private async Task<int> readChar_(bool peek = false)
    {
        if (bufferPosition >= bufferSize)
        {
            if (endOfFile)
                return -1;
                
            bufferSize = await reader!.ReadAsync(buffer, 0, buffer.Length);
            bufferPosition = 0;
                
            if (bufferSize == 0)
            {
                endOfFile = true;
                return -1;
            }
        }
            
        char result = buffer[bufferPosition];
        if (!peek)
            bufferPosition++;
        
        return result;
    }
        
    private Task<int> readChar()
    {
        return readChar_();
    }

    private Task<int> peekChar()
    {
        return readChar_(peek: true);
    }
        
    protected virtual async Task<List<String?>?> readRow(int rowNumber)
    {
        List<String?> row = new ();
        stringBuilder.Clear();
        if (endOfFile)
            return null;            
            
        int num;
        CsvState state = CsvState.StartOfLine;
        while ((num = await readChar()) != -1)
        {
            switch (state)
            {                    
                case CsvState.Data:
                case CsvState.StartOfLine:
                case CsvState.Seeking:
                {
                    if (num == '\n')
                    {
                        updateStreamInformation(rowNumber, "\n");
                        if (state != CsvState.StartOfLine)
                            row.Add(stringBuilder.ToString());                                    
                        return CsvUtil.trimRow(row, settings.trimStyle);
                    }
                    else if (num == '\r')
                    {
                        if (await peekChar() == '\n')
                        {
                            await readChar();            // consumes both \r\n
                            updateStreamInformation(rowNumber, "\r\n");                            
                        }
                        else
                            updateStreamInformation(rowNumber, "\r");
                        
                        if (state != CsvState.StartOfLine)
                            row.Add(stringBuilder.ToString());                                    
                        return CsvUtil.trimRow(row, settings.trimStyle);
                    }
                    else if (num == settings.delimiter)
                    {
                        row.Add(stringBuilder.ToString());
                        stringBuilder.Clear();
                        state = CsvState.Seeking;                                
                    }
                    else if (num == settings.escapeChar)
                    {
                        if (state == CsvState.Data)
                        {
                            if (settings.allowStrayQuotes)
                                stringBuilder.Append(settings.escapeChar);
                            else 
                                throw new IllegalStateException($"Line {rowNumber + 1} contains a quote that isn't wrapped with quotes"); 
                        }
                        else
                            state = CsvState.Quoted;
                    }
                    else
                    {
                        state = CsvState.Data;
                        stringBuilder.Append((char)num);
                    }
                    break;
                }

                case CsvState.Quoted:
                {
                    if (num == settings.escapeChar)
                    {
                        int next = await peekChar();
                        if (next == settings.escapeChar)
                        {
                            await readChar();            // consumes second quote
                            stringBuilder.Append(settings.escapeChar);
                        }
                        else if (next == settings.delimiter)
                        {
                            row.Add(stringBuilder.ToString());
                            stringBuilder.Clear();
                            await readChar();            // consumes comma
                            state = CsvState.Seeking;                                
                        }
                        else if (next == '\n' || next == '\r' || next == -1)
                        {
                            state = CsvState.Data;
                        }
                        else
                        {
                            if (settings.allowTextAfterClosingQuote)
                                state = CsvState.Data;
                            else
                                throw new IllegalStateException($"Line {rowNumber + 1} contains an unexpected character {(char)next} after quote");
                        }
                    }
                    else
                    {
                        stringBuilder.Append((char)num);
                    }
                    break;
                }
            }                
        }            

        endOfFile = true;
        updateStreamInformation(rowNumber, null);

        switch (state)
        {
            case CsvState.StartOfLine:
                if (rowNumber > 0)
                    streamInformation!.endsWithNewLine = true;
                return null;
                
            case CsvState.Quoted:
                if (!settings.terminateQuoteOnEndOfFile)
                    throw new IllegalStateException("File ends with open quotes");
                break;
        }
            
        row.Add(stringBuilder.ToString());                                    
        return CsvUtil.trimRow(row, settings.trimStyle);
    }

    private void updateStreamInformation(int lineNumber, String? newLine)
    {
        if (lineNumber > 0)
            return;

        streamInformation!.updateStreamNewLine(newLine);
        streamInformation.updateStreamEncoding(reader!.CurrentEncoding);
    }

    public void setSource(StreamInformation streamInformation_, IStreamFactory streamFactory_)
    {
        streamInformation = streamInformation_;
        streamFactory = streamFactory_;
    }

    public void setNextRowProcessor(IRowProcessor next)
    {
        rowProcessor = next;
    }

    public async ValueTask DisposeAsync()
    {
        if (reader != null)                    
            reader.Dispose();
            
        reader = null;

        if (streamFactory is IAsyncDisposable sfDisposable)
            await sfDisposable.DisposeAsync();

        streamFactory = null;
    }
}