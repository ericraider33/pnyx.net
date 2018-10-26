using pnyx.net.api;

namespace pnyx.net.shims
{
    public class RowBufferingShim : IRowBuffering
    {
        
        
        public string[][] bufferingRow(string[] row)
        {
            throw new System.NotImplementedException();
        }

        public string[][] endOfFile()
        {
            throw new System.NotImplementedException();
        }
    }
}