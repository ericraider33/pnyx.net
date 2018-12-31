using System;
using System.Collections.Generic;

namespace pnyx.net.api
{
    public interface IRowBuffering
    {
        List<String> rowHeader(List<String> header);        
        List<List<String>> bufferingRow(List<String> row);
        List<List<String>> endOfFile();
    }
}