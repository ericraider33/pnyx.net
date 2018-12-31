using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.shims
{
    public class RowTransformerFunc : IRowTransformer
    {        
        public Func<List<String>,List<String>> rowTransformerFunc;
        public bool treatHeaderAsRow;

        public List<String> transformHeader(List<String> header)
        {
            if (treatHeaderAsRow)
                return rowTransformerFunc(header);
            else
                return header;
        }

        public List<String> transformRow(List<String> row)
        {
            return rowTransformerFunc(row);
        }
    }
}