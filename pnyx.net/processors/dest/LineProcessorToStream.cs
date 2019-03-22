using System;
using System.IO;
using pnyx.net.util;

namespace pnyx.net.processors.dest
{
    public class LineProcessorToStream : ILineProcessor, IDisposable
    {
        public Stream stream { get; private set; }
        public TextWriter writer { get; private set; }
        public readonly StreamInformation streamInformation;

        private String previousLine;
        private bool closed;
        
        public LineProcessorToStream(StreamInformation streamInformation, Stream stream)
        {
            this.stream = stream;
            this.streamInformation = streamInformation;
        }

        public void processLine(String line)
        {
            if (previousLine != null)
            {
                writer.Write(previousLine);
                writer.Write(streamInformation.getOutputNewline());
            }
            else
            {
                writer = new StreamWriter(stream, streamInformation.getOutputEncoding());
            }

            previousLine = line;
        }

        public void endOfFile()
        {
            if (previousLine != null)
            {              
                writer.Write(previousLine);
                if (streamInformation.endsWithNewLine)
                    writer.Write(streamInformation.getOutputNewline());
            }
            else
            {
                writer = new StreamWriter(stream, streamInformation.getOutputEncoding());                        
            }

            previousLine = null;
            writer.Flush();
            
            writer.Close();
            closed = true;
        }

        public void Dispose()
        {
            if (writer != null)
            {
                if (!closed)
                    writer.Flush();
                
                writer.Dispose();
            }

            stream = null;
            writer = null;
        }
    }
}