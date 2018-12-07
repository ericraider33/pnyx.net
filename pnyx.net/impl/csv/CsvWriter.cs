using System;
using System.IO;
using System.Text;
using pnyx.net.errors;
using pnyx.net.util;

namespace pnyx.net.impl.csv
{
    public class CsvWriter : RowToCsvStream
    {
        public CsvWriter(Stream stream, Encoding defaultEncoding = null) : 
            base(fromDefaultEncoding(defaultEncoding), stream)
        {
            writer = new StreamWriter(stream, streamInformation.encoding);
        }

        private static StreamInformation fromDefaultEncoding(Encoding defaultEncoding = null)
        {
            StreamInformation result = new StreamInformation();
            if (defaultEncoding != null)
                result.defaultEncoding = defaultEncoding;

            result.encoding = result.defaultEncoding;    // advances "default" to "actual"
            
            return result;
        }        

        public override void processRow(string[] row)
        {
            throw new IllegalStateException("Use writeRow method instead");            
        }

        public override void endOfFile()
        {
            throw new IllegalStateException("Use writeRow method instead");            
        }

        public void writeRow(String[] row)
        {
            writeRow_(row);
            writer.WriteLine();
        }
    }
}