using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.shims
{
    public class RowTransformerShimAnd : IRowTransformer
    {
        public ILineTransformer lineTransformer;

        public List<String> transformHeader(List<String> header)
        {
            return header;
        }

        public List<String> transformRow(List<String> row)
        {
            List<String> result = new List<String>();
            foreach (String original in row)
            {
                String column = lineTransformer.transformLine(original);
                if (column == null)
                    return null;

                result.Add(column);                
            }

            return result;
        }
    }
}