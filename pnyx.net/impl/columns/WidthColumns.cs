using System;
using pnyx.net.api;
using pnyx.net.util;

namespace pnyx.net.impl.columns
{
    public class WidthColumns : IRowTransformer
    {
        public int columns;
        public String pad = ""; 
        
        public string[] transformRow(string[] row)
        {
            return RowHelper.fixWidth(row, columns, pad);
        }
    }
}