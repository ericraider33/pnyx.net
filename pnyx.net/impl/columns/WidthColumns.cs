using System;
using pnyx.net.api;
using pnyx.net.util;

namespace pnyx.net.impl.columns
{
    public class WidthColumns : IRowTransformer
    {
        public int columns;
        public String pad = "";

        public String[] transformHeader(String[] header)
        {
            header = RowUtil.fixWidth(header, columns, "");
            RowUtil.setDefaultHeaderNames(header);
            return header;
        }

        public String[] transformRow(String[] row)
        {
            return RowUtil.fixWidth(row, columns, pad);
        }
    }
}