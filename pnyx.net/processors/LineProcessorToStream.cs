using System;
using System.IO;
using pnyx.net.util;

namespace pnyx.net.processors
{
    public class LineProcessorToStream : ILineProcessor, IDisposable
    {
        public Stream stream { get; private set; }
        public TextWriter writer { get; private set; }
        public readonly StreamInformation streamInformation;

        private String previousLine;
        
        public LineProcessorToStream(StreamInformation streamInformation, Stream stream)
        {
            this.stream = stream;
            this.streamInformation = streamInformation;
        }

        public void processLine(string line)
        {
            if (previousLine != null)
            {
                writer.Write(previousLine);
                writer.Write(streamInformation.newLine);
            }
            else
            {
                writer = new StreamWriter(stream, streamInformation.encoding);
            }

            previousLine = line;
        }

        public void endOfFile()
        {
            if (previousLine != null)
            {              
                writer.Write(previousLine);
                if (streamInformation.endsWithNewLine)
                    writer.Write(streamInformation.newLine);
            }
            else
            {
                writer = new StreamWriter(stream, streamInformation.encoding);                        
            }

            previousLine = null;
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
    }
}