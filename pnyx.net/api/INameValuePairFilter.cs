using System;
using System.Collections.Generic;

namespace pnyx.net.api;

public interface INameValuePairFilter
{
    bool shouldKeepPair(IDictionary<String,Object> pairs);
}