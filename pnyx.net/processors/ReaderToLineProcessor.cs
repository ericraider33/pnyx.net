using System;
using System.IO;

namespace pnyx.net.processors
{
    public class ReaderToLineProcessor : IProcessor, IDisposable
    {
        public TextReader reader;
        public ILineProcessor lineProcessor;

        public ReaderToLineProcessor()
        {            
        }
        
        public ReaderToLineProcessor(TextReader reader, ILineProcessor lineProcessor)
        {
            this.reader = reader;
            this.lineProcessor = lineProcessor;
        }
        
        public ReaderToLineProcessor(Stream stream, ILineProcessor lineProcessor)
        {
            reader = new StreamReader(stream);
            this.lineProcessor = lineProcessor;
        }

        public void process()
        {
            String line;
            while ((line = reader.ReadLine()) != null)
            {
                lineProcessor.process(line);
            }
            lineProcessor.endOfFile();
        }

        public void Dispose()
        {
            if (reader != null)                    
                reader.Dispose();
            
            reader = null;
        }
    }
}