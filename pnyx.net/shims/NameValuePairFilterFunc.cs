using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.shims;

public class NameValuePairFilterFunc : INameValuePairFilter
{
    public Func<IDictionary<string, object>, bool> pairFilterFunc;
    
    public bool shouldKeepPair(IDictionary<string, object> pairs)
    {
        return pairFilterFunc(pairs);
    }
}