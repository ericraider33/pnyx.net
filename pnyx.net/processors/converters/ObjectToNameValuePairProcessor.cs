using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.processors.converters;

public class ObjectToNameValuePairProcessor : IObjectProcessor, INameValuePairPart
{
    public IObjectConverterFromNameValuePair converter;
    public INameValuePairProcessor processor;
    
    public void processObject(object obj)
    {
        IDictionary<string, Object> nameValuePair = converter.objectToNameValuePair(obj);
        processor.processNameValuePair(nameValuePair);
    }

    public void endOfFile()
    {
        processor.endOfFile();
    }

    public void setNextNameValuePairProcessor(INameValuePairProcessor next)
    {
        processor = next;
    }
}