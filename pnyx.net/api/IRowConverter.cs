using System;
using System.IO;
using pnyx.net.processors;
using pnyx.net.util;

namespace pnyx.net.api
{
    public interface IRowConverter
    {
        String[] lineToRow(String line);
        String rowToLine(String[] row);       
        
        IRowProcessor buildRowDestination(StreamInformation streamInformation, Stream stream);
    }
}