using System;

namespace pnyx.net.api
{
    public interface IRowConverter
    {
        String[] lineToRow(String line);
        String rowToLine(String[] row);        
    }
}