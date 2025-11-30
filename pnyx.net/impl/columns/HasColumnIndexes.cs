using System;
using System.Collections.Generic;
using System.Linq;
using pnyx.net.api;

namespace pnyx.net.impl.columns;

public class HasColumnIndexes : IRowFilter
{
    public readonly bool verifyColumnHasText;
    public readonly HashSet<ColumnIndex> columnIndexes;
    private readonly ColumnIndex maxColumnNumber;
    
    /// <summary>
    /// Function to perform a check whether content is present or not. Return True to keep row; false to reject / skip row 
    /// </summary>
    private readonly Func<string, bool> checkContent;

    public HasColumnIndexes(IEnumerable<ColumnIndex> columns, bool verifyColumnHasText = true, Func<string, bool> checkContent = null)
    {
        this.columnIndexes = new HashSet<ColumnIndex>(columns);
        this.verifyColumnHasText = verifyColumnHasText;
        this.checkContent = checkContent;

        maxColumnNumber = columnIndexes.Max(x => x);
    }

    public bool shouldKeepRow(List<String> row)
    {
        if (row.Count < (maxColumnNumber.Index + 1))
            return false;

        if (verifyColumnHasText)
        {
            foreach (ColumnIndex columnIndex in columnIndexes)
            {
                String column = row[columnIndex.Index];
                if (String.IsNullOrEmpty(column))
                    return false;
            }

            return true;
        }
        
        if (checkContent != null)
        {
            foreach (ColumnIndex columnIndex in columnIndexes)
            {
                string column = row[columnIndex.Index];
                if (!checkContent(column))
                    return false;
            }

            return true;
        }
            
        return true;
    }
}