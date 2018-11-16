using System;
using pnyx.net.api;

namespace pnyx.net.impl
{
    public class Columns : IRowTransformer
    {
        public int[] indexes;                                // zero-based
        
        public String[] transformRow(String[] row)
        {
            String[] result = new String[indexes.Length];
            for (int i = 0; i < indexes.Length; i++)
            {
                int columnIndex = indexes[i];
                String column = columnIndex < row.Length ? row[columnIndex] : "";
                result[i] = column;
            }

            return result;
        }
    }
}