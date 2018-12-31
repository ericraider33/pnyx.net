using System;
using System.Collections.Generic;
using System.Linq;
using pnyx.net.api;

namespace pnyx.net.impl.columns
{
    public class HasColumns : IRowFilter
    {
        public readonly bool verifyColumnHasText;
        public readonly HashSet<int> columnNumbers;
        private readonly int maxColumnNumber;

        public HasColumns(IEnumerable<int> columns, bool verifyColumnHasText = true)
        {
            this.columnNumbers = new HashSet<int>(columns);
            this.verifyColumnHasText = verifyColumnHasText;

            maxColumnNumber = columnNumbers.Max(x => x);
        }

        public bool shouldKeepRow(String[] row)
        {
            if (row.Length < maxColumnNumber)
                return false;

            if (!verifyColumnHasText)
                return true;

            foreach (int columnNumber in columnNumbers)
            {
                String column = row[columnNumber - 1];
                if (String.IsNullOrEmpty(column))
                    return false;
            }

            return true;
        }
    }
}