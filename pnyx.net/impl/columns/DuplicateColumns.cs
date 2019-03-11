using System;
using System.Collections.Generic;
using pnyx.net.api;
using pnyx.net.util;

namespace pnyx.net.impl.columns
{
    public class DuplicateColumns : IRowTransformer
    {
        public readonly HashSet<int> columnNumbers;

        public DuplicateColumns(IEnumerable<int> columns)
        {
            columnNumbers = new HashSet<int>(columns);
        }

        public List<String> transformHeader(List<String> header)
        {
            return RowUtil.duplicateColumns(header, columnNumbers);
        }

        public List<String> transformRow(List<String> row)
        {
            return RowUtil.duplicateColumns(row, columnNumbers);
        }
    }
}