using pnyx.net.api;

namespace pnyx.net.processors.objects;

public class ObjectTransformerProcessor : IObjectPart, IObjectProcessor
{
    public IObjectTransformer transformer;
    public IObjectProcessor processor;
    
    public void setNextObjectProcessor(IObjectProcessor next)
    {
        processor = next;
    }

    public void processObject(object obj)
    {
        obj = transformer.transformObject(obj);
        if (obj != null)
            processor.processObject(obj);
    }

    public void endOfFile()
    {
        processor.endOfFile();
    }
}
