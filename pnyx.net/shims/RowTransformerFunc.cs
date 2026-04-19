using System;
using System.Collections.Generic;
using System.Linq;
using pnyx.net.api;
using pnyx.net.util;

namespace pnyx.net.shims;

public class RowTransformerFunc : IRowTransformer
{        
    public Func<List<String?>, List<String?>?> rowTransformerFunc;
    public bool treatHeaderAsRow;

    public RowTransformerFunc(Func<List<string?>, List<string?>?> rowTransformerFunc, bool treatHeaderAsRow = false)
    {
        this.rowTransformerFunc = rowTransformerFunc;
        this.treatHeaderAsRow = treatHeaderAsRow;
    }

    public List<String>? transformHeader(List<String> header)
    {
        if (!treatHeaderAsRow)
            return header;

        List<string?> asRow = header.Cast<string?>().ToList();
        List<string?>? result = rowTransformerFunc(asRow);
        if (result == null)
            return null;

        return result.toHeader();
    }

    public List<String?>? transformRow(List<String?> row)
    {
        return rowTransformerFunc(row);
    }
}