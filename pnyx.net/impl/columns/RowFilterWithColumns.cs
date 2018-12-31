using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl.columns
{
    public class RowFilterWithColumns : IRowFilter
    {
        public int[] indexes { get; private set; }   
        public IRowFilter rowFilter  { get; private set; }

        private readonly List<String> subColumns;
        
        public RowFilterWithColumns(int[] indexes, IRowFilter rowFilter)
        {
            this.indexes = indexes;
            this.rowFilter = rowFilter;
            
            subColumns = new List<String>(indexes.Length);
        }

        public bool shouldKeepRow(List<String> row)
        {
            subColumns.Clear();
            foreach (int columnIndex in indexes)
            {
                String column = columnIndex < row.Count ? row[columnIndex] : "";
                subColumns.Add(column);
            }

            return rowFilter.shouldKeepRow(subColumns);
        }
    }
}