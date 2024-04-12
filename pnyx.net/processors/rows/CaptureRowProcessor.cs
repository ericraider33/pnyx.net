using System;
using System.Collections.Generic;

namespace pnyx.net.processors.rows;

public class CaptureRowProcessor : IRowProcessor
{
    public List<String> header { get; private set; }
    public List<List<String>> rows { get; }
    public bool eof { get; private set; }

    public CaptureRowProcessor()
    {
        rows = new List<List<String>>();        
    }

    public void rowHeader(List<String> header)
    {
        this.header = header;
    }

    public void processRow(List<String> row)
    {
        rows.Add(row);
    }

    public void endOfFile()
    {
        eof = true;
    }
}