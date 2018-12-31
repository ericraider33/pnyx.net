using System;
using pnyx.net.api;

namespace pnyx.net.shims
{
    public class RowTransformerShimOr : IRowTransformer
    {
        public ILineTransformer lineTransformer;

        public String[] transformHeader(String[] header)
        {
            return header;
        }
        
        public String[] transformRow(String[] row)
        {
            bool keep = false;
            String[] result = new String[row.Length];
            for (int i = 0; i < row.Length; i++)
            {
                String column = lineTransformer.transformLine(row[i]);
                if (column == null)
                {
                    result[i] = "";
                }
                else
                {
                    result[i] = column;
                    keep = true;
                }
            }
            
            return keep ? result : null;
        }
    }
}