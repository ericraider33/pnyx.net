using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.shims;

public class NameValuePairTransformerFunc : INameValuePairTransformer
{
    public Func<IDictionary<String, Object>, IDictionary<String, Object>> transformFunc;
    
    public IDictionary<string, object> transformPairs(IDictionary<string, object> pairs)
    {
        return transformFunc(pairs);
    }
}