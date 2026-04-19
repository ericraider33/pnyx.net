using System.Threading.Tasks;
using pnyx.net.api;

namespace pnyx.net.processors.objects;

public class ObjectFilterProcessor : IObjectPart, IObjectProcessor
{
    public IObjectFilter filter { get; }
    public IObjectProcessor? processor { get; private set; }
    
    public ObjectFilterProcessor(IObjectFilter filter)
    {
        this.filter = filter;
    }
    
    public void setNextObjectProcessor(IObjectProcessor next)
    {
        processor = next;
    }

    public async Task processObject(object obj)
    {
        if (filter.shouldKeepObject(obj))
            await processor!.processObject(obj);
    }

    public async Task endOfFile()
    {
        await processor!.endOfFile();
    }
}