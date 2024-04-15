using System;
using System.Collections.Generic;

namespace pnyx.net.api;

public interface IObjectConverterFromRow
{
    Object rowToObject(List<String> row, List<String> header = null);
    List<String> objectToRow(Object obj);

    /// <summary>
    /// Called on the first object to generate a header row, if desired.
    /// NOTE: The same object is also passed to objectToRow method for conversion to a row
    /// </summary>
    /// <returns>Header names or NULL if no header should be generated</returns>
    List<String> objectToHeader(Object obj);
}