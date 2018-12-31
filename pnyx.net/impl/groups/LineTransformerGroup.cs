using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl.groups
{
    public class LineTransformerGroup : ILineTransformer
    {
        public readonly List<ILineTransformer> transformers = new List<ILineTransformer>();
        
        public String transformLine(String line)
        {
            foreach (ILineTransformer transformer in transformers)
            {
                line = transformer.transformLine(line);
                if (line == null)
                    return null;                
            }
            
            return line;            
        }
    }
}