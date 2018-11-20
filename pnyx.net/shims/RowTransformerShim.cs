using System;
using pnyx.net.api;

namespace pnyx.net.shims
{
    public class RowTransformerShim : IRowTransformer
    {
        public ILineTransformer lineTransformer;
        
        public string[] transformRow(string[] row)
        {
            bool keep = false;
            string[] result = new string[row.Length];
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