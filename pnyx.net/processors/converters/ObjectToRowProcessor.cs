using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using pnyx.net.api;

namespace pnyx.net.processors.converters;

public class ObjectToRowProcessor : IObjectProcessor, IRowPart
{
    public IObjectConverterFromRow converter { get; }
    public IRowProcessor? processor { get; private set; }
    private int lineNumber;

    public ObjectToRowProcessor(IObjectConverterFromRow converter)
    {
        this.converter = converter;
    }

    public async Task processObject(object obj)
    {
        lineNumber++;

        if (lineNumber == 1)
        {
            List<String>? header = converter.objectToHeader(obj);
            if (header != null)
                await processor!.rowHeader(header);

            lineNumber++;
        }
        
        List<String?> row = converter.objectToRow(obj);
        await processor!.processRow(row);
    }

    public async Task endOfFile()
    {
        await processor!.endOfFile();
    }

    public void setNextRowProcessor(IRowProcessor next)
    {
        processor = next;
    }
}