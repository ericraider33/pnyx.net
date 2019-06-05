using System;
using System.Collections.Generic;
using System.IO;
using pnyx.net.processors;
using pnyx.net.util;

namespace pnyx.net.impl.csv
{
    public class RowToCsvStream : IRowProcessor, IDisposable
    {
        public Stream stream { get; private set; }
        public TextWriter writer { get; private set; }
        public StreamInformation streamInformation { get; }
        public CsvSettings settings { get; }

        private List<String> previousRow;
                
        public RowToCsvStream(
            StreamInformation streamInformation, 
            Stream stream,
            CsvSettings settings
            )
        {
            this.stream = stream;
            this.streamInformation = streamInformation;
            this.settings = settings;
        }

        public void rowHeader(List<String> header)
        {
            processRow(header);
        }

        public void processRow(List<String> row)
        {
            if (previousRow != null)
            {
                CsvUtil.writeRow(writer, previousRow, settings.delimiter, settings.escapeChar, settings.charsNeedEscape);
                writer.Write(streamInformation.getOutputNewline());
            }
            else
            {
                writer = new StreamWriter(stream, streamInformation.getOutputEncoding());
            }

            previousRow = row;
        }

        public void endOfFile()
        {
            if (previousRow != null)
            {              
                CsvUtil.writeRow(writer, previousRow, settings.delimiter, settings.escapeChar, settings.charsNeedEscape);
                if (streamInformation.endsWithNewLine)
                    writer.Write(streamInformation.getOutputNewline());
            }
            else
            {
                writer = new StreamWriter(stream, streamInformation.getOutputEncoding());                        
            }

            previousRow = null;
            writer.Flush();
        }

        public void Dispose()
        {
            if (writer != null)
            {
                writer.Flush();
                writer.Dispose();
            }
            writer = null;

            if (stream != null)
                stream.Dispose();
            stream = null;
        }
    }
}