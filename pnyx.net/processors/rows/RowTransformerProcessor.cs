using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using pnyx.net.api;

namespace pnyx.net.processors.rows;

public class RowTransformerProcessor : IRowPart, IRowProcessor
{
    public IRowTransformer transform { get; }
    public IRowProcessor? processor { get; private set; }

    public RowTransformerProcessor(IRowTransformer transform)
    {
        this.transform = transform;
    }

    public async Task rowHeader(List<String> header)
    {
        List<string>? transformed = transform.transformHeader(header);
        if (transformed != null)
            await processor!.rowHeader(transformed);            
    }

    public async Task processRow(List<String?> row)
    {
        List<string?>? transformed = transform.transformRow(row);
        if (transformed != null)
            await processor!.processRow(transformed);
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