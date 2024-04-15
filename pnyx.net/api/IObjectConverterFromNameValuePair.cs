using System;
using System.Collections.Generic;

namespace pnyx.net.api;

public interface IObjectConverterFromNameValuePair
{
    Object nameValuePairToObject(IDictionary<string, Object> row);
    IDictionary<string, Object> objectToNameValuePair(Object obj);
}