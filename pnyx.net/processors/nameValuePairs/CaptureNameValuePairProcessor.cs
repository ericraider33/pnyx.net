using System;
using System.Collections.Generic;

namespace pnyx.net.processors.nameValuePairs;

public class CaptureNameValuePairProcessor : INameValuePairProcessor
{
    public List<IDictionary<String, Object>> records { get; } = new();
    public bool eof { get; private set; }

    public void processNameValuePair(IDictionary<string, Object> record)
    {
        records.Add(record);
    }

    public void endOfFile()
    {
        eof = true;
    }
}