using System;
using System.Collections.Generic;
using System.Linq;

namespace pnyx.net.util
{
    public static class RowUtil
    {
        public static List<String> replaceColumn(List<String> row, int columnNumber, params String[] replacement)
        {
            int columnIndex = columnNumber - 1;
            if (columnIndex > row.Count)
                return row;
            
            if (columnIndex < row.Count)
                row.RemoveAt(columnIndex);
            
            for (int i = 0; i < replacement.Length; i++)
                row.Insert(columnIndex+i, replacement[i]);
                        
            return row;
        }
        
        public static List<String> insertColumns(List<String> row, int columnNumber, params String[] replacement)
        {
            int columnIndex = columnNumber - 1;
            if (columnIndex > row.Count)
                return row;
            
            for (int i = 0; i < replacement.Length; i++)
                row.Insert(columnIndex+i, replacement[i]);
                        
            return row;
        }

        public static List<String> insertBlankColumns(List<String> row, ISet<int> columnNumbers, String pad = "")
        {
            int finalSize = row.Count + columnNumbers.Count;
            List<String> result = new List<String>(finalSize);
            for (int source = 0; result.Count < finalSize; source++)
            {
                if (columnNumbers.Contains(source + 1))
                    result.Add(pad);

                if (result.Count < finalSize)
                    result.Add(row[source]);
            }

            return result;
        }

        public static List<String> duplicateColumns(List<String> row, ISet<int> columnNumbers, String pad = "")
        {
            int finalSize = row.Count + columnNumbers.Count;
            List<String> result = new List<String>(finalSize);
            for (int source = 0; result.Count < finalSize; source++)
            {
                if (columnNumbers.Contains(source + 1))
                    result.Add(source < row.Count ? row[source] : pad);

                if (result.Count < finalSize)
                    result.Add(row[source]);
            }
            
            return result;
        }

        public static List<String> removeColumns(List<String> row, ISet<int> columnNumbers)
        {
            List<String> result = new List<String>(row.Count);
            for (int i = 0; i < row.Count; i++)
            {
                if (columnNumbers.Contains(i + 1))
                    continue;

                result.Add(row[i]);
            }

            return result;
        }

        public static List<String> fixWidth(List<String> row, int columns, String pad = "")
        {
            List<String> result = new List<String>(columns);

            for (int i = 0; i < columns; i++)
            {
                if (i < row.Count)
                    result.Add(row[i]);
                else
                    result.Add(pad);
            }
                        
            return result;
        }

        public static bool isEqual(List<String> rowA, List<String> rowB)
        {
            if (rowA == null && rowB == null)
                return true;
            else if (rowA == null || rowB == null)
                return false;
            else
                return rowA.SequenceEqual(rowB);
        }

        public static List<String> setDefaultHeaderNames(List<String> header)
        {
            for (int i = 0; i < header.Count; i++)
            {
                if (String.IsNullOrEmpty(header[i]))
                    header[i] = "Header" + (i + 1);
            }

            return header;
        }

        public static List<String> asRow(this String[] source)
        {
            if (source == null)
                return null;

            return new List<String>(source);
        }
    }
}