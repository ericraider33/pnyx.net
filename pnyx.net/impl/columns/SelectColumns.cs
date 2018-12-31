using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl.columns
{
    public class SelectColumns : IRowTransformer
    {
        public int[] indexes;                                // zero-based

        public List<String> transformHeader(List<String> header)
        {
            return transformRow(header);
        }

        public List<String> transformRow(List<String> row)
        {
            List<String> result = new List<String>(indexes.Length);
            foreach (int columnIndex in indexes)
            {
                String column = columnIndex < row.Count ? row[columnIndex] : "";
                result.Add(column);
            }

            return result;
        }
    }
}