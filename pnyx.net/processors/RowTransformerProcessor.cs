using pnyx.net.api;

namespace pnyx.net.processors
{
    public class RowTransformerProcessor : IRowProcessor
    {
        public IRowTransformer transform;
        public IRowProcessor processor;

        public void processRow(string[] row)
        {
            row = transform.transformRow(row);
            processor.processRow(row);
        }

        public void endOfFile()
        {
            processor.endOfFile();
        }
    }
}