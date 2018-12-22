using System;
using System.Collections.Generic;
using pnyx.net.api;
using pnyx.net.util;

namespace pnyx.net.impl.columns
{
    public class InsertColumns : IRowTransformer
    {
        public String pad = "";
        public readonly HashSet<int> columnNumbers;

        public InsertColumns(IEnumerable<int> columns)
        {
            columnNumbers = new HashSet<int>(columns);
        }

        public String[] transformHeader(String[] header)
        {
            header = RowUtil.insertBlankColumns(header, columnNumbers, "");
            return RowUtil.setDefaultHeaderNames(header);
        }

        public String[] transformRow(String[] row)
        {
            return RowUtil.insertBlankColumns(row, columnNumbers, pad);
        }
    }
}