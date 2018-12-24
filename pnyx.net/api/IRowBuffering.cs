using System;

namespace pnyx.net.api
{
    public interface IRowBuffering
    {
        String[] rowHeader(String[] header);        
        String[][] bufferingRow(String[] row);
        String[][] endOfFile();
    }
}