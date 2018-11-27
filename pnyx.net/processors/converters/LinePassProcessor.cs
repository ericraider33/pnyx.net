namespace pnyx.net.processors.converters
{
    public class LinePassProcessor : ILineProcessor, ILinePart
    {
        public ILineProcessor processor;
        
        public void processLine(string line)
        {
            processor.processLine(line);
        }

        public void endOfFile()
        {
            processor.endOfFile();
        }

        public void setNext(ILineProcessor next)
        {
            processor = next;
        }
    }
}