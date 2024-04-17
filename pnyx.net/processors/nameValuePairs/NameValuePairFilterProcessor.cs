using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.processors.nameValuePairs;

public class NameValuePairFilterProcessor : INameValuePairPart, INameValuePairProcessor
{
    public INameValuePairFilter filter;
    public INameValuePairProcessor processor;
    
    public void setNextNameValuePairProcessor(INameValuePairProcessor next)
    {
        processor = next;
    }

    public void processNameValuePair(IDictionary<string, object> record)
    {
        if (filter.shouldKeepPair(record))
            processor.processNameValuePair(record);
    }

    public void endOfFile()
    {
        processor.endOfFile();
    }
}