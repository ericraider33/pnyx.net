using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl.columns
{
    public class RowTransformerWithColumns : IRowTransformer
    {
        public int[] indexes { get; private set; }   
        public IRowTransformer rowTransformer  { get; private set; }

        public RowTransformerWithColumns(int[] indexes, IRowTransformer rowTransformer)
        {
            this.indexes = indexes;
            this.rowTransformer = rowTransformer;            
        }

        public List<String> transformHeader(List<String> header)
        {
            List<String> subColumns = subColumnsIn(header);
            List<String> transformed = rowTransformer.transformHeader(subColumns);
            return subColumnsOut(transformed, header);
        }

        public List<String> transformRow(List<String> row)
        {
            List<String> subColumns = subColumnsIn(row);
            List<String> transformed = rowTransformer.transformRow(subColumns);
            return subColumnsOut(transformed, row);
        }

        private List<String>  subColumnsIn(List<String> row)
        {
            List<String> subColumns = new List<String>(indexes.Length);
            foreach (int columnIndex in indexes)
            {
                String column = columnIndex < row.Count ? row[columnIndex] : "";
                subColumns.Add(column);
            }
            return subColumns;
        }

        private List<String> subColumnsOut(List<String> transformed, List<String> row)
        {
            for (int i = 0; i < indexes.Length; i++)
            {
                int columnIndex = indexes[i];
                if (columnIndex >= row.Count)
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