using System;
using pnyx.net.api;

namespace pnyx.net.impl.columns
{
    public class RowTransformerWithColumns : IRowTransformer
    {
        public int[] indexes { get; private set; }   
        public IRowTransformer rowTransformer  { get; private set; }

        private String[] subColumns;
        
        public RowTransformerWithColumns(int[] indexes, IRowTransformer rowTransformer)
        {
            this.indexes = indexes;
            this.rowTransformer = rowTransformer;
            
            subColumns = new String[indexes.Length];
        }

        public string[] transformRow(string[] row)
        {
            for (int i = 0; i < indexes.Length; i++)
            {
                int columnIndex = indexes[i];
                String column = columnIndex < row.Length ? row[columnIndex] : "";
                subColumns[i] = column;
            }

            String[] transformed = rowTransformer.transformRow(subColumns);
            
            for (int i = 0; i < indexes.Length; i++)
            {
                int columnIndex = indexes[i];
                if (columnIndex < row.Length)
                    row[columnIndex] = transformed[i];
            }

            return row;
        }
    }
}