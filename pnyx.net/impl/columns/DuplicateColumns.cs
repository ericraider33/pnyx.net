using System;
using System.Collections.Generic;
using pnyx.net.api;
using pnyx.net.util;

namespace pnyx.net.impl.columns;

public class DuplicateColumns : IRowTransformer
{
    public readonly HashSet<ColumnIndex> columnIndices;

    public DuplicateColumns(IEnumerable<ColumnIndex> columns)
    {
        columnIndices = new HashSet<ColumnIndex>(columns);
    }

    public List<String> transformHeader(List<String> header)
    {
        return RowUtil.duplicateColumns(header, columnIndices);
    }

    public List<String> transformRow(List<String> row)
    {
        return RowUtil.duplicateColumns(row, columnIndices);
    }
}