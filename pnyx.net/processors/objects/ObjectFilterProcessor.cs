using pnyx.net.api;

namespace pnyx.net.processors.objects;

public class ObjectFilterProcessor : IObjectPart, IObjectProcessor
{
    public IObjectFilter filter;
    public IObjectProcessor processor;
    
    public void setNextObjectProcessor(IObjectProcessor next)
    {
        processor = next;
    }

    public void processObject(object obj)
    {
        if (filter.shouldKeepObject(obj))
            processor.processObject(obj);
    }

    public void endOfFile()
    {
        processor.endOfFile();
    }
}