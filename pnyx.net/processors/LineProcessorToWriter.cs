using System;
using System.IO;

namespace pnyx.net.processors
{
    public class LineProcessorToWriter : ILineProcessor, IDisposable
    {
        public TextWriter writer;

        public LineProcessorToWriter()
        {            
        }
        
        public LineProcessorToWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        public LineProcessorToWriter(Stream stream)
        {
            writer = new StreamWriter(stream);
        }

        public void process(string line)
        {
            writer.WriteLine(line);
        }

        public void endOfFile()
        {
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
        }
    }
}