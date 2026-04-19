using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pnyx.net.processors.sources;

public class RowProcessorFunc : IRowPart, IProcessor
{
    public Func<List<String>?>? header  { get; }        
    public Func<IEnumerable<List<String?>>> source { get; }
    public IRowProcessor? processor { get; private set; }

    public RowProcessorFunc(Func<List<string>>? header, Func<IEnumerable<List<string?>>> source)
    {
        this.header = header;
        this.source = source;
    }

    public void setNextRowProcessor(IRowProcessor next)
    {
        this.processor = next;
    }

    public async Task process()
    {
        if (header != null)
        {
            List<String>? headerData = header();
            if (headerData != null)
                await processor!.rowHeader(headerData);
        }

        IEnumerable<List<String?>> data = source();
        foreach (List<String?> row in data)
            await processor!.processRow(row);
            
        await processor!.endOfFile();
    }
}