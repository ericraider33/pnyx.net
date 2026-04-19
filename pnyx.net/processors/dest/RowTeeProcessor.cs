using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pnyx.net.processors.dest;

public class RowTeeProcessor : IRowProcessor, IRowPart
{
    public IRowProcessor? processor { get; private set; }
    public IRowProcessor tee { get; }

    public RowTeeProcessor(IRowProcessor tee)
    {
        this.tee = tee;
    }

    public async Task rowHeader(List<String> header)
    {
        await processor!.rowHeader(header);
        await tee.rowHeader(header);            
    }

    public async Task processRow(List<String?> row)
    {
        await processor!.processRow(row);
        await tee.processRow(row);
    }

    public async Task endOfFile()
    {
        await processor!.endOfFile();
        await tee.endOfFile();
    }

    public void setNextRowProcessor(IRowProcessor next)
    {
        processor = next;
    }
}