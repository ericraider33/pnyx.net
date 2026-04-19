using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using pnyx.net.errors;
using pnyx.net.fluent;

namespace pnyx.net.impl.csv;

public class CsvWriter : IAsyncDisposable
{
    public TextWriter? writer { get; private set; }
    public char delimiter { get; private set; }
    public char escapeChar { get; private set; }
    public char[] charsNeedEscape { get; private set; }          
        
    public CsvWriter
    (
        TextWriter writer,
        char? delimiter = null,
        char? escapeChar = null
    )
    {
        this.writer = writer;
        this.delimiter = delimiter ?? Settings.DEFAULT_CSV_DELIMITER;
        this.escapeChar = escapeChar ?? Settings.DEFAULT_CSV_ESCAPE_CHAR;
        charsNeedEscape = CsvUtil.createCharsNeedEscape(this.delimiter, this.escapeChar);
    }
        
    public CsvWriter
    (
        Stream stream, 
        Encoding? defaultEncoding = null,
        char? delimiter = null,
        char? escapeChar = null
    )
    {
        defaultEncoding = defaultEncoding ?? Settings.DEFAULT_ENCODING;
        writer = new StreamWriter(stream, defaultEncoding);
        this.delimiter = delimiter ?? Settings.DEFAULT_CSV_DELIMITER;
        this.escapeChar = escapeChar ?? Settings.DEFAULT_CSV_ESCAPE_CHAR;
        charsNeedEscape = CsvUtil.createCharsNeedEscape(this.delimiter, this.escapeChar);
    }
    
    public async Task writeRow(IEnumerable<String?> row)
    {
        if (writer == null)
            throw new IllegalStateException("Writer has been disposed");

        await CsvUtil.writeRowAsync(writer, row, delimiter, escapeChar, charsNeedEscape);
        await writer.WriteLineAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (writer != null)
        {
            await writer.FlushAsync();
            await writer.DisposeAsync();
        }
        writer = null;
    }
}