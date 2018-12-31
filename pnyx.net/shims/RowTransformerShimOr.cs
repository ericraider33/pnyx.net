using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.shims
{
    public class RowTransformerShimOr : IRowTransformer
    {
        public ILineTransformer lineTransformer;

        public List<String> transformHeader(List<String> header)
        {
            return header;
        }
        
        public List<String> transformRow(List<String> row)
        {
            bool keep = false;
            List<String> result = new List<String>(row.Count);
            foreach (String original in row)
            {
                String column = lineTransformer.transformLine(original);
                if (column == null)
                {
                    result.Add("");                
                }
                else
                {
                    result.Add(column);                
                    keep = true;
                }

            }
            
            return keep ? result : null;
        }
    }
}