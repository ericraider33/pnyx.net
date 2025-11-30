using System;
using System.Collections.Generic;
using pnyx.net.api;
using pnyx.net.util;

namespace pnyx.net.impl.columns;

public class InsertColumns : IRowTransformer
{
    public String pad = "";
    public readonly HashSet<ColumnIndex> columnIndices;

    public InsertColumns(IEnumerable<ColumnIndex> columns)
    {
        columnIndices = new HashSet<ColumnIndex>(columns);
    }

    public List<String> transformHeader(List<String> header)
    {
        header = RowUtil.insertBlankColumns(header, columnIndices, "");
        return RowUtil.setDefaultHeaderNames(header);
    }

    public List<String> transformRow(List<String> row)
    {
        return RowUtil.insertBlankColumns(row, columnIndices, pad);
    }
}