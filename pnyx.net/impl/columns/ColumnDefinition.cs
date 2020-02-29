using System;
using System.Collections.Generic;
using System.Linq;
using pnyx.net.api;
using pnyx.net.util;

namespace pnyx.net.impl.columns
{
    public class ColumnInformation
    {
        public String header;
        public int maxWidth;
        public int minWidth;
        public bool nullable;

        public ColumnInformation()
        {
            minWidth = Int32.MaxValue;   
        }
    }
    
    public class ColumnDefinition : IRowBuffering
    {
        [Flags]
        public enum Flags
        {
            None = 0,
            MaxWidth = 1,
            MinWidth = 2,
            Nullable = 4,
            Header = 8,
            All = MaxWidth | MinWidth | Nullable | Header
        }
        
        public StreamInformation streamInformation { get; private set; }
        public int limit;
        public Flags flag;
        
        private int lineNumber;
        private List<ColumnInformation> infoList = new List<ColumnInformation>();

        public ColumnDefinition(StreamInformation streamInformation)
        {
            this.streamInformation = streamInformation;
            limit = Int32.MaxValue;
            flag = Flags.All;
        }

        public List<String> rowHeader(List<String> header)
        {
            lineNumber++;

            for (int i = infoList.Count; i < header.Count; i++)
                infoList.Add(new ColumnInformation());

            for (int i = 0; i < header.Count; i++)
            {
                String column = header[i];
                ColumnInformation info = infoList[i];

                if (flag.HasFlag(Flags.Header))
                    info.header = column;
            }

            return null;
        }

        public List<List<String>> bufferingRow(List<String> row)
        {
            if (lineNumber == 0 && flag.HasFlag(Flags.Header))
            {
                rowHeader(row);
                return null;
            }
            
            lineNumber++;

            for (int i = infoList.Count; i < row.Count; i++)
                infoList.Add(new ColumnInformation());
                
            for (int i = 0; i < row.Count; i++)
            {
                String column = row[i];
                ColumnInformation info = infoList[i];

                if (lineNumber == 1 && flag.HasFlag(Flags.Header))
                {
                    info.header = column;
                    continue;
                }
                
                if (flag.HasFlag(Flags.MaxWidth))
                    info.maxWidth = Math.Max(info.maxWidth, column.Length);

                if (flag.HasFlag(Flags.Nullable) || flag.HasFlag(Flags.MinWidth))
                    info.minWidth = Math.Min(info.minWidth, column.Length);
            }
            
            if (lineNumber >= limit)
                streamInformation.active = false;

            return null;
        }

        public List<List<String>> endOfFile()
        {
            List<List<String>> result = new List<List<String>>();

            if (flag.HasFlag(Flags.Header))
                result.Add(buildOutput("Header", list => list.Select(ci => ci.header)));

            if (flag.HasFlag(Flags.MinWidth))
                result.Add(buildOutput("MinWidth", list => list.Select(ci => ci.minWidth.ToString())));

            if (flag.HasFlag(Flags.MaxWidth))
                result.Add(buildOutput("MaxWidth", list => list.Select(ci => ci.maxWidth.ToString())));

            if (flag.HasFlag(Flags.Nullable))
                result.Add(buildOutput("Nullable", list => list.Select(ci => ci.minWidth > 0 ? "not null" : "null")));
            
            return result;
        }

        private List<String> buildOutput(String title, Func<List<ColumnInformation>, IEnumerable<String>> action)
        {
            List<String> row = new List<String>(infoList.Count + 1);
            row.Add(title);
            row.AddRange(action(infoList));
            return row;            
        }
    }
}