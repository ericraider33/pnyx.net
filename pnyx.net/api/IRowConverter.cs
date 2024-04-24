using System;
using System.Collections.Generic;
using System.IO;
using pnyx.net.processors;
using pnyx.net.util;

namespace pnyx.net.api;

public interface IRowConverter
{
    List<String> lineToRow(String line);
    String rowToLine(List<String> row);       
        
    IRowProcessor buildRowDestination(StreamInformation streamInformation, Stream stream);
}