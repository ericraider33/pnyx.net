using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pnyx.net.processors.rows;

public class CaptureRowProcessor : IRowProcessor
{
    public List<String>? header { get; private set; }
    public List<List<String?>> rows { get; }
    public bool eof { get; private set; }

    public CaptureRowProcessor()
    {
        rows = new List<List<String?>>();        
    }

    public Task rowHeader(List<String> rowHeader)
    {
        header = rowHeader;
        return Task.CompletedTask;
    }

    public Task processRow(List<String?> row)
    {
        rows.Add(row);
        return Task.CompletedTask;
    }

    public Task endOfFile()
    {
        eof = true;
        return Task.CompletedTask;
    }
}