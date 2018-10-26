using pnyx.net.api;

namespace pnyx.net.shims
{
    public class RowTransformerShim : IRowTransformer
    {
        public ILineTransformer lineTransformer;
        
        public string[] transformRow(string[] row)
        {
            string[] result = new string[row.Length];
            for (int i = 0; i < row.Length; i++)                
                result[i] = lineTransformer.transformLine(row[i]);

            return result;
        }
    }
}