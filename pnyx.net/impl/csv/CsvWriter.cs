using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using pnyx.net.fluent;

namespace pnyx.net.impl.csv
{
    public class CsvWriter : IDisposable
    {
        public TextWriter writer { get; private set; }
        public char delimiter { get; private set; }
        public char escapeChar { get; private set; }
        public char[] charsNeedEscape { get; private set; }          
        
        public CsvWriter(
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
        
        public CsvWriter(
            Stream stream, 
            Encoding defaultEncoding = null,
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

        public void Dispose()
        {
            if (writer != null)
            {
                writer.Flush();
                writer.Dispose();
            }
            writer = null;
        }
        
        public void writeRow(IEnumerable<String> row)
        {
            CsvUtil.writeRow(writer, row, delimiter, escapeChar, charsNeedEscape);
            writer.WriteLine();
        }
    }
}