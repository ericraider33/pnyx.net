using System.Collections.Generic;
using System.Threading.Tasks;
using pnyx.net.api;

namespace pnyx.net.processors.nameValuePairs;

public class NameValuePairFilterProcessor : INameValuePairPart, INameValuePairProcessor
{
    public INameValuePairFilter filter { get; }
    public INameValuePairProcessor? processor { get; private set; }

    public NameValuePairFilterProcessor(INameValuePairFilter filter)
    {
        this.filter = filter;
    }

    public void setNextNameValuePairProcessor(INameValuePairProcessor next)
    {
        processor = next;
    }

    public async Task processNameValuePair(IDictionary<string, object?> record)
    {
        if (filter.shouldKeepPair(record))
            await processor!.processNameValuePair(record);
    }

    public async Task endOfFile()
    {
        await processor!.endOfFile();
    }
}