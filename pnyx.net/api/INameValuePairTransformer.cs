using System;
using System.Collections.Generic;

namespace pnyx.net.api;

public interface INameValuePairTransformer
{
    IDictionary<String, Object> transformPairs(IDictionary<String, Object> pairs);
}