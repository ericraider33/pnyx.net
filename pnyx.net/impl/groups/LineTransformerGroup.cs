using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl.groups;

public class LineTransformerGroup : ILineTransformer
{
    public readonly List<ILineTransformer> transformers = new ();
        
    public String? transformLine(String line)
    {
        string? result = line;
        foreach (ILineTransformer transformer in transformers)
        {
            result = transformer.transformLine(result);
            if (result == null)
                return null;                
        }

        return result;
    }
}