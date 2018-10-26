using System;

namespace pnyx.net.api
{
    public interface IRowBuffering
    {
        String[][] bufferingRow(String[] row);
        String[][] endOfFile();
    }
}