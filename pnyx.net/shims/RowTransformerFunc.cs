using System;
using pnyx.net.api;

namespace pnyx.net.shims
{
    public class RowTransformerFunc : IRowTransformer
    {
        public Func<String[],String[]> rowTransformerFunc;
        
        public string[] transformRow(string[] row)
        {
            return rowTransformerFunc(row);
        }
    }
}