using System;
using System.IO;
using System.Text;
using pnyx.net.api;
using pnyx.net.processors;
using pnyx.net.util;

namespace pnyx.net.impl
{
    public class DelimiterRowConverter : IRowConverter
    {
        public String delimiter;
        private readonly StringBuilder builder = new StringBuilder();
        
        public String[] lineToRow(String line)
        {
            return line.Split(delimiter);
        }

        public String rowToLine(String[] row)
        {
            builder.Clear();
            if (row.Length == 0)
                return "";

            builder.Append(row[0]);
            for (int i = 1; i < row.Length; i++)
            {
                builder.Append(delimiter);
                builder.Append(row[i]);
            }

            return builder.ToString();
        }

        public IRowProcessor buildRowDestination(StreamInformation streamInformation, Stream stream)
        {
            return null;
        }
    }
}