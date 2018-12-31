using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl.columns
{
    public class HeaderNames : IRowTransformer
    {
        public readonly Dictionary<int, String> nameMap;

        public HeaderNames(Dictionary<int, String> nameMap)
        {
            this.nameMap = nameMap;
        }

        public List<String> transformHeader(List<String> header)
        {
            foreach (KeyValuePair<int, String> namePair in nameMap)
            {
                int index = namePair.Key;
                String name = namePair.Value;
                if (index < header.Count)
                    header[index] = name;
            }

            return header;
        }

        public List<String> transformRow(List<String> row)
        {
            return row;
        }
    }
}