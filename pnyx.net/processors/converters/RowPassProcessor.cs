using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pnyx.net.processors.converters;

public class RowPassProcessor : IRowProcessor, IRowPart
{
    public IRowProcessor? processor { get; private set; }

    public async Task rowHeader(List<String> header)
    {
        await processor!.rowHeader(header);
    }

    public async Task processRow(List<String?> row)
    {
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