using System;
using pnyx.net.api;

namespace pnyx.net.shims
{
    public class RowTransformerShimAnd : IRowTransformer
    {
        public ILineTransformer lineTransformer;
        
        public string[] transformRow(string[] row)
        {
            bool keep = true;
            string[] result = new string[row.Length];
            for (int i = 0; i < row.Length; i++)
            {
                String column = lineTransformer.transformLine(row[i]);
                if (column == null)
                {
                    result[i] = "";
                    keep = false;
                }
                else
                {
                    result[i] = column;
                }
            }
            
            return keep ? result : null;
        }
    }
}