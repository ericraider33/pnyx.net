using System;
using System.Collections.Generic;
using pnyx.net.api;
using pnyx.net.util;

namespace pnyx.net.impl.columns
{
    public class RemoveColumns: IRowTransformer
    {
        public readonly HashSet<int> columnNumbers;

        public RemoveColumns(IEnumerable<int> columns)
        {
            columnNumbers = new HashSet<int>(columns);
        }

        public List<String> transformHeader(List<String> header)
        {
            return RowUtil.removeColumns(header, columnNumbers);
        }

        public List<String> transformRow(List<String> row)
        {
            return RowUtil.removeColumns(row, columnNumbers);
        }
    }
}