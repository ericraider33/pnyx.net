using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using pnyx.net.processors;

namespace pnyx.net.impl.columns;

public class ColumnToLine : IRowProcessor, ILinePart
{
    public ColumnIndex index { get; }
    public ILineProcessor? processor { get; set; }

    public ColumnToLine(ColumnIndex columnIndex)
    {
        this.index = columnIndex;
    }

    public async Task rowHeader(List<String> header)
    {
        String line = index < header.Count ? header[index] : "";
        await processor!.processLine(line);
    }

    public async Task processRow(List<String?> row)
    {
        string line = index < row.Count ? (row[index] ?? "") : "";
        await processor!.processLine(line);
    }

    public async Task endOfFile()
    {
        await processor!.endOfFile();
    }

    public void setNextLineProcessor(ILineProcessor next)
    {
        processor = next;
    }
}