using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl.groups;

public class RowTransformerGroup : IRowTransformer
{
    public readonly List<IRowTransformer> transformers = new ();

    public List<String> transformHeader(List<String> header)
    {
        return header;
    }

    public List<String?>? transformRow(List<String?> row)
    {
        List<String?>? result = row;
        foreach (IRowTransformer transformer in transformers)
        {
            result = transformer.transformRow(result);
            if (result == null)
                return null;                
        }
            
        return result;            
    }
}