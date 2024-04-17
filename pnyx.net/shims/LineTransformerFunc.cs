using System;
using pnyx.net.api;

namespace pnyx.net.shims;

public class LineTransformerFunc : ILineTransformer
{
    public Func<String, String> lineTransformerFunc;
        
    public String transformLine(String line)
    {
        return lineTransformerFunc(line);
    }
}