using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl.groups
{
    public class RowTransformerGroup : IRowTransformer
    {
        public readonly List<IRowTransformer> transformers = new List<IRowTransformer>();

        public List<String> transformHeader(List<String> header)
        {
            return header;
        }

        public List<String> transformRow(List<String> row)
        {
            foreach (IRowTransformer transformer in transformers)
            {
                row = transformer.transformRow(row);
                if (row == null)
                    return null;                
            }
            
            return row;            
        }
    }
}