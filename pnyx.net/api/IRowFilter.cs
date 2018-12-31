using System;
using System.Collections.Generic;

namespace pnyx.net.api
{
    public interface IRowFilter
    {
        bool shouldKeepRow(List<String> row);
    }
}