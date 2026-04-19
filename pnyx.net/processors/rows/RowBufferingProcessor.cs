using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using pnyx.net.api;

namespace pnyx.net.processors.rows;

public class RowBufferingProcessor : IRowPart, IRowProcessor
{
    public IRowBuffering buffering { get; }
    public IRowProcessor? processor { get; private set; }

    public RowBufferingProcessor(IRowBuffering buffering)
    {
        this.buffering = buffering;
    }

    public async Task rowHeader(List<string> header)
    {
        List<string>? transformed = buffering.rowHeader(header);
        if (transformed != null)
            await processor!.rowHeader(transformed);
    }

    public async Task processRow(List<String?> row)
    {
        await forward(buffering.bufferingRow(row));
    }

    public async Task endOfFile()
    {
        await forward(buffering.endOfFile());
        await processor!.endOfFile();
    }

    private async Task forward(List<List<String?>>? rows)
    {
        if (rows == null)
            return;
            
        foreach (List<String?> row in rows)
            await processor!.processRow(row);
    }

    public void setNextRowProcessor(IRowProcessor next)
    {
        processor = next;
    }        
}