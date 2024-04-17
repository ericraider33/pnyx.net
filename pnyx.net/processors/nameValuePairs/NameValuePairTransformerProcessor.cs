using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.processors.nameValuePairs;

public class NameValuePairTransformerProcessor : INameValuePairPart, INameValuePairProcessor
{
    public INameValuePairTransformer transformer;
    public INameValuePairProcessor processor;
    
    public void setNextNameValuePairProcessor(INameValuePairProcessor next)
    {
        processor = next;
    }

    public void processNameValuePair(IDictionary<string, object> record)
    {
        record = transformer.transformPairs(record);
        if (record != null)
            processor.processNameValuePair(record);
    }

    public void endOfFile()
    {
        processor.endOfFile();
    }
}