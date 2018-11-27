namespace pnyx.net.processors.converters
{
    public class RowPassProcessor : IRowProcessor, IRowPart
    {
        public IRowProcessor processor;
        
        public void processRow(string[] row)
        {
            processor.processRow(row);
        }

        public void endOfFile()
        {
            processor.endOfFile();
        }

        public void setNext(IRowProcessor next)
        {
            processor = next;
        }
    }
}