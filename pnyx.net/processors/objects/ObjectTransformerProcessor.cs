using System.Threading.Tasks;
using pnyx.net.api;

namespace pnyx.net.processors.objects;

public class ObjectTransformerProcessor : IObjectPart, IObjectProcessor
{
    public IObjectTransformer transformer { get; }
    public IObjectProcessor? processor { get; private set; }

    public ObjectTransformerProcessor(IObjectTransformer transformer)
    {
        this.transformer = transformer;
    }

    public void setNextObjectProcessor(IObjectProcessor next)
    {
        processor = next;
    }

    public async Task processObject(object obj)
    {
        object? result = transformer.transformObject(obj);
        if (result != null)
            await processor!.processObject(result);
    }

    public async Task endOfFile()
    {
        await processor!.endOfFile();
    }
}
