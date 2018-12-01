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
        
        public string[] transformRow(string[] row)
        {
            return RowHelper.insertBlankColumns(row, columnNumbers, pad);
        }
    }
}