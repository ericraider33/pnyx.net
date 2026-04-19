using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using pnyx.net.api;

namespace pnyx.net.processors.rows;

public class RowFilterProcessor : IRowPart, IRowProcessor
{
    public IRowFilter filter { get; }
    public IRowProcessor? processor { get; private set; }
    
    public RowFilterProcessor(IRowFilter filter)
    {
        this.filter = filter;
    }

    public async Task rowHeader(List<String> header)
    {
        await processor!.rowHeader(header);
    }

    public async Task processRow(List<String?> row)
    {
        if (filter.shouldKeepRow(row))
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