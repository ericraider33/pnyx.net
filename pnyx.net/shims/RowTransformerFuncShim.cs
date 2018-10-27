using System;
using pnyx.net.api;

namespace pnyx.net.shims
{
    public class RowTransformerFuncShim : IRowTransformer
    {
        public Func<String,String> lineTransformer;
        
        public string[] transformRow(string[] row)
        {
            bool keep = false;
            string[] result = new string[row.Length];
            for (int i = 0; i < row.Length; i++)
            {
                String value = lineTransformer(row[i]);
                if (value == null)
                {
                    result[i] = "";
                }
                else
                {
                    result[i] = value;
                    keep = true;
                }
            }
            
            return keep ? result : null;
        }
    }
}