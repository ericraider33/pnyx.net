using System;
using System.Collections.Generic;

namespace pnyx.net.processors.sources;

public class ObjectProcessorFunc : IObjectPart, IProcessor
{
    public Func<IEnumerable<object>> source { get; private set; }
    public IObjectProcessor next { get; private set; }
    
    public ObjectProcessorFunc(Func<IEnumerable<object>> source)
    {
        this.source = source;
    }
    
    public void setNextObjectProcessor(IObjectProcessor next)
    {
        this.next = next;
    }

    public void process()
    {
        IEnumerable<object> data = source();
        foreach (object obj in data)
            next.processObject(obj);
            
        next.endOfFile();
    }
}