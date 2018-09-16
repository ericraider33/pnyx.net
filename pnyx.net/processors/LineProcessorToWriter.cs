using System.IO;

namespace pnyx.net.processors
{
    public class LineProcessorToWriter : ILineProcessor
    {
        public TextWriter writer;
        
        public void process(string line)
        {
            writer.WriteLine(line);
        }

        public void endOfFile()
        {
        }
    }
}