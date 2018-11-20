using System;
using System.IO;
using pnyx.net.util;

namespace pnyx.net.processors
{
    public class RowToCsvStream : IRowProcessor, IDisposable
    {
        public Stream stream { get; private set; }
        public TextWriter writer { get; private set; }
        public readonly StreamInformation streamInformation;

        private String[] previousRow;
        
        public RowToCsvStream(StreamInformation streamInformation, Stream stream)
        {
            this.stream = stream;
            this.streamInformation = streamInformation;
        }

        public void processRow(string[] row)
        {
            if (previousRow != null)
            {
                writeRow(previousRow);
                writer.Write(streamInformation.getNewline());
            }
            else
            {
                writer = new StreamWriter(stream, streamInformation.encoding);
            }

            previousRow = row;
        }

        public void endOfFile()
        {
            if (previousRow != null)
            {              
                writeRow(previousRow);
                if (streamInformation.endsWithNewLine)
                    writer.Write(streamInformation.getNewline());
            }
            else
            {
                writer = new StreamWriter(stream, streamInformation.encoding);                        
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

            stream = null;
            writer = null;
        }
        
        protected virtual void writeRow(String[] row)
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