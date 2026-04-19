using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using pnyx.net.api;

namespace pnyx.net.processors.converters;

public class NameValuePairToObjectProcessor : INameValuePairProcessor, IObjectPart
{
    public IObjectConverterFromNameValuePair converter { get; }
    public IObjectProcessor? processor { get; private set; }

    public NameValuePairToObjectProcessor(IObjectConverterFromNameValuePair converter)
    {
        this.converter = converter;
    }

    public async Task processNameValuePair(IDictionary<string, object?> record)
    {
        Object obj = converter.nameValuePairToObject(record);
        await processor!.processObject(obj);
    }

    public async Task endOfFile()
    {
        await processor!.endOfFile();
    }

    public void setNextObjectProcessor(IObjectProcessor next)
    {
        processor = next;
    }
}