using System;
using System.Collections.Generic;

namespace pnyx.net.api
{
    public interface IRowTransformer
    {
        List<String> transformHeader(List<String> header);
        List<String> transformRow(List<String> row);
    }
}