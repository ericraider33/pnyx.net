using System;
using System.Collections.Generic;
using pnyx.net.api;
using pnyx.net.util;

namespace pnyx.net.impl.columns
{
    public class WidthColumns : IRowTransformer
    {
        public int columns;
        public String pad = "";

        public List<String> transformHeader(List<String> header)
        {
            header = RowUtil.fixWidth(header, columns, "");
            RowUtil.setDefaultHeaderNames(header);
            return header;
        }

        public List<String> transformRow(List<String> row)
        {
            return RowUtil.fixWidth(row, columns, pad);
        }
    }
}