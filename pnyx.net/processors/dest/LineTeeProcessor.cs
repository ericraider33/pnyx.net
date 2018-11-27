namespace pnyx.net.processors.dest
{
    public class LineTeeProcessor : ILineProcessor, ILinePart
    {
        public ILineProcessor processor;
        public ILineProcessor tee { get; private set; }

        public LineTeeProcessor(ILineProcessor tee)
        {
            this.tee = tee;
        }

        public void processLine(string line)
        {
            processor.processLine(line);
            tee.processLine(line);
        }

        public void endOfFile()
        {
            processor.endOfFile();
            tee.endOfFile();
        }

        public void setNext(ILineProcessor next)
        {
            processor = next;
        }
    }
}