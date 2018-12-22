using System;
using System.Collections.Generic;
using pnyx.net.api;
using pnyx.net.util;

namespace pnyx.net.impl.columns
{
    public class DuplicateColumns : IRowTransformer
    {
        public String pad = "";
        public readonly HashSet<int> columnNumbers;

        public DuplicateColumns(IEnumerable<int> columns)
        {
            columnNumbers = new HashSet<int>(columns);
        }

        public String[] transformHeader(String[] header)
        {
            header = RowUtil.duplicateColumns(header, columnNumbers, pad);
            return RowUtil.setDefaultHeaderNames(header);
        }

        public String[] transformRow(String[] row)
        {
            return RowUtil.duplicateColumns(row, columnNumbers, pad);
        }
    }
}