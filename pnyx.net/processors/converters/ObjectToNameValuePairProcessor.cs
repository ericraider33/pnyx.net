using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using pnyx.net.api;

namespace pnyx.net.processors.converters;

public class ObjectToNameValuePairProcessor : IObjectProcessor, INameValuePairPart
{
    public IObjectConverterFromNameValuePair converter { get; }
    public INameValuePairProcessor? processor { get; private set; }
    
    public ObjectToNameValuePairProcessor(IObjectConverterFromNameValuePair converter)
    {
        this.converter = converter;
    }
    
    public async Task processObject(object obj)
    {
        IDictionary<string, Object?> nameValuePair = converter.objectToNameValuePair(obj);
        await processor!.processNameValuePair(nameValuePair);
    }

    public async Task endOfFile()
    {
        await processor!.endOfFile();
    }

    public void setNextNameValuePairProcessor(INameValuePairProcessor next)
    {
        processor = next;
    }
}