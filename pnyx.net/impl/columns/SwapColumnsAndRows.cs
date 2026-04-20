using System;
using System.Collections.Generic;
using pnyx.net.api;
using pnyx.net.util;

namespace pnyx.net.impl.columns;

public class SwapColumnsAndRows : IRowBuffering
{
    private readonly List<List<String?>> buffer = new ();
    private int lineNumber;

    public List<String>? rowHeader(List<String> header)
    {
        bufferingRow(header.toRow());
        return null;
    }

    public List<List<String?>>? bufferingRow(List<String?> row)
    {
        lineNumber++;

        for (int i = buffer.Count; i < row.Count; i++)
        {
            List<String?> outRow = new ();
            buffer.Add(outRow);

            for (int j = outRow.Count; j < lineNumber - 1; j++)
                outRow.Add("");
        }

        for (int i = 0; i < buffer.Count; i++)
        {
            String? column = i < row.Count ? row[i] : "";
            List<String?> outRow = buffer[i];
            outRow.Add(column);
        }

        return null;
    }

    public List<List<String?>> endOfFile()
    {
        return buffer;
    }
}