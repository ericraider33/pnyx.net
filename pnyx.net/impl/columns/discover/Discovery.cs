using System;
using System.Collections.Generic;
using System.Linq;
using pnyx.net.api;

namespace pnyx.net.impl.columns.discover
{
    public class Discovery : IRowBuffering
    {
        public int firstPass = 1000;
        public int validatePass = 10000;
        public Examine examine = new Examine();

        private readonly List<List<string>> buffer = new List<List<string>>();
        private int columns = 0;
        private readonly List<DataDescriptor> descriptors = new List<DataDescriptor>();
        
        public List<string> rowHeader(List<string> header)
        {
            return header;
        }

        public List<List<string>> bufferingRow(List<string> row)
        {
            buffer.Add(row);
            columns = Math.Max(columns, row.Count);
            
            if (buffer.Count == firstPass)
                examineData();
            
            return null;
        }

        public List<List<string>> endOfFile()
        {
            if (buffer.Count < firstPass)
                examineData();

            List<String> row = descriptors.Select(desc => desc.ToString()).ToList();
            return new List<List<string>> { row };
        }

        private void examineData()
        {
            List<String> data = new List<String>(firstPass);
            
            for (int colIndex = 0; colIndex < columns; colIndex++)
            {
                data.Clear();

                for (int rowIndex = 0; rowIndex < buffer.Count && rowIndex < firstPass; rowIndex++)
                {
                    List<String> row = buffer[rowIndex];
                    if (colIndex >= row.Count)
                        continue;

                    String value = row[colIndex];
                    if (String.IsNullOrEmpty(value))
                        continue;
                    
                    data.Add(value);
                }
                
                if (data.Count == 0)
                    descriptors.Add(new DataDescriptor());
                else
                    descriptors.Add(examine.examine(data));
            }
        }
    }
}