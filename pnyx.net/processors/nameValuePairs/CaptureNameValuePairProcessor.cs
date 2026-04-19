using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pnyx.net.processors.nameValuePairs;

public class CaptureNameValuePairProcessor : INameValuePairProcessor
{
    public List<IDictionary<String, Object?>> records { get; } = new();
    public bool eof { get; private set; }

    public Task processNameValuePair(IDictionary<string, Object?> record)
    {
        records.Add(record);
        return Task.CompletedTask;
    }

    public Task endOfFile()
    {
        eof = true;
        return Task.CompletedTask;
    }
}