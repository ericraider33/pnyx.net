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

        public String[] transformHeader(String[] header)
        {
            subColumnsIn(header);
            String[] transformed = rowTransformer.transformHeader(subColumns);
            return subColumnsOut(transformed, header);
        }

        public String[] transformRow(String[] row)
        {
            subColumnsIn(row);
            String[] transformed = rowTransformer.transformRow(subColumns);
            return subColumnsOut(transformed, row);
        }

        private void subColumnsIn(String[] row)
        {
            for (int i = 0; i < indexes.Length; i++)
            {
                int columnIndex = indexes[i];
                String column = columnIndex < row.Length ? row[columnIndex] : "";
                subColumns[i] = column;
            }            
        }

        private String[] subColumnsOut(String[] transformed, String[] row)
        {
            for (int i = 0; i < indexes.Length; i++)
            {
                int columnIndex = indexes[i];
                if (columnIndex >= row.Length)
                    continue;

                if (transformed == null)
                    row[columnIndex] = "";
                else
                    row[columnIndex] = transformed[i];
            }

            return row;
        }
    }
}