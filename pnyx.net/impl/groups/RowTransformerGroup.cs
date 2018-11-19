using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl.groups
{
    public class RowTransformerGroup : IRowTransformer
    {
        public readonly List<IRowTransformer> transformers = new List<IRowTransformer>();

        public string[] transformRow(string[] row)
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