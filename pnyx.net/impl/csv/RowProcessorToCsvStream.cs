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
        public TextWriter writer { get; protected set; }
        public readonly StreamInformation streamInformation;

        private List<String> previousRow;
        
        public RowToCsvStream(StreamInformation streamInformation, Stream stream)
        {
            this.stream = stream;
            this.streamInformation = streamInformation;
        }

        public virtual void rowHeader(List<String> header)
        {
            processRow(header);
        }

        public virtual void processRow(List<String> row)
        {
            if (previousRow != null)
            {
                writeRow_(previousRow);
                writer.Write(streamInformation.getOutputNewline());
            }
            else
            {
                writer = new StreamWriter(stream, streamInformation.getOutputEncoding());
            }

            previousRow = row;
        }

        public virtual void endOfFile()
        {
            if (previousRow != null)
            {              
                writeRow_(previousRow);
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
        
        protected virtual void writeRow_(List<String> row)
        {
            bool first = true;
            foreach (String val in row)
            {
                if (!first)
                    writer.Write(",");
                first = false;

                if (val == null)
                    continue;

                if (val.IndexOfAny(new char[] { ',', '"', '\n', '\r', '\t', ' ' }) >= 0)
                {
                    writer.Write('"');
                    foreach (char c in val)
                    {
                        if (c == '"')
                            writer.Write("\"\"");
                        else
                            writer.Write(c);
                    }
                    writer.Write('"');
                }
                else
                    writer.Write(val);
            }
        }
    }
}