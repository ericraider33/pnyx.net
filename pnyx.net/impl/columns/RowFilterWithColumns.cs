using System;
using pnyx.net.api;

namespace pnyx.net.impl.columns
{
    public class RowFilterWithColumns : IRowFilter
    {
        public int[] indexes { get; private set; }   
        public IRowFilter rowFilter  { get; private set; }

        private String[] subColumns;
        
        public RowFilterWithColumns(int[] indexes, IRowFilter rowFilter)
        {
            this.indexes = indexes;
            this.rowFilter = rowFilter;
            
            subColumns = new String[indexes.Length];
        }

        public bool shouldKeepRow(String[] row)
        {
            for (int i = 0; i < indexes.Length; i++)
            {
                int columnIndex = indexes[i];
                String column = columnIndex < row.Length ? row[columnIndex] : "";
                subColumns[i] = column;
            }

            return rowFilter.shouldKeepRow(subColumns);
        }
    }
}