using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pnyx.net.processors.sources;

public class ObjectProcessorFunc : IObjectPart, IProcessor
{
    public Func<IEnumerable<object>> source { get; }
    public IObjectProcessor? processor { get; private set; }
    
    public ObjectProcessorFunc(Func<IEnumerable<object>> source)
    {
        this.source = source;
    }
    
    public void setNextObjectProcessor(IObjectProcessor next)
    {
        this.processor = next;
    }

    public async Task process()
    {
        IEnumerable<object> data = source();
        foreach (object obj in data)
            await processor!.processObject(obj);
            
        await processor!.endOfFile();
    }
}