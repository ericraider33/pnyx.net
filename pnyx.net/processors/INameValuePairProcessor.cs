using System;
using System.Collections.Generic;

namespace pnyx.net.processors;

public interface INameValuePairProcessor
{
    void processNameValuePair(IDictionary<string, Object> record);
    void endOfFile();
}