using System;
using pnyx.net.api;

namespace pnyx.net.shims
{
    public class RowTransformerFunc : IRowTransformer
    {        
        public Func<String[],String[]> rowTransformerFunc;
        public bool treatHeaderAsRow;

        public String[] transformHeader(String[] header)
        {
            if (treatHeaderAsRow)
                return rowTransformerFunc(header);
            else
                return header;
        }

        public String[] transformRow(String[] row)
        {
            return rowTransformerFunc(row);
        }
    }
}