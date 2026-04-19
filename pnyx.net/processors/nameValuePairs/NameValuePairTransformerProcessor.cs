using System.Collections.Generic;
using System.Threading.Tasks;
using pnyx.net.api;

namespace pnyx.net.processors.nameValuePairs;

public class NameValuePairTransformerProcessor : INameValuePairPart, INameValuePairProcessor
{
    public INameValuePairTransformer transformer { get; }
    public INameValuePairProcessor? processor { get; private set; }

    public NameValuePairTransformerProcessor(INameValuePairTransformer transformer)
    {
        this.transformer = transformer;
    }

    public void setNextNameValuePairProcessor(INameValuePairProcessor next)
    {
        processor = next;
    }

    public async Task processNameValuePair(IDictionary<string, object?> record)
    {
        IDictionary<string, object?>? result = transformer.transformPairs(record);
        if (result != null)
            await processor!.processNameValuePair(result);
    }

    public async Task endOfFile()
    {
        await processor!.endOfFile();
    }
}