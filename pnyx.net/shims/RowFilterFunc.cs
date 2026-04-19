using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.shims;

public class RowFilterFunc : IRowFilter
{
    public Func<List<String?>,bool> rowFilterFunc { get; }

    public RowFilterFunc(Func<List<string?>, bool> rowFilterFunc)
    {
        this.rowFilterFunc = rowFilterFunc;
    }

    public bool shouldKeepRow(List<String?> row)
    {
        return rowFilterFunc(row);
    }        
}