using System;

namespace pnyx.net.util
{
    public static class RowHelper
    {
        public static String[] replaceColumn(String[] row, int columnNumber, params String[] replacement)
        {
            String[] result = new String[row.Length - 1 + replacement.Length];

            int columnIndex = columnNumber - 1;
            for (int i = 0; i < row.Length; i++)
            {
                if (i < columnIndex)
                    result[i] = row[i];
                else if (i > columnIndex)
                    result[i + replacement.Length - 1] = row[i];
                else
                {
                    for (int j = 0; j < replacement.Length; j++)
                        result[i + j] = replacement[j];
                }
            }
                        
            return result;
        }
        
        public static String[] insertColumns(String[] row, int columnNumber, params String[] replacement)
        {
            int columnIndex = columnNumber - 1;
            String[] result = new String[Math.Max(row.Length,columnIndex) + replacement.Length];

            for (int i = 0; i < Math.Min(row.Length, columnIndex); i++)
                result[i] = row[i];

            for (int i = 0; i < replacement.Length; i++)
                result[i + columnIndex] = replacement[i];
            
            for (int i = Math.Min(row.Length, columnIndex); i < row.Length; i++)
                result[i + replacement.Length] = row[i];
                        
            return result;
        }

        public static String[] fixWidth(String[] row, int columns, String pad = "")
        {
            String[] result = new String[columns];

            for (int i = 0; i < result.Length; i++)
            {
                if (i < row.Length)
                    result[i] = row[i];
                else
                    result[i] = pad;
            }
                        
            return result;
        }
    }
}