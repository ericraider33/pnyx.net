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
        
        public string[] transformRow(string[] row)
        {
            return RowHelper.removeColumns(row, columnNumbers);
        }
    }
}