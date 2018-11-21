using System;

namespace pnyx.net.api
{
    public interface IRowFilter
    {
        bool shouldKeepRow(String[] row);
    }
}