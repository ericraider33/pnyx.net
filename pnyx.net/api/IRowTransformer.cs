using System;

namespace pnyx.net.api
{
    public interface IRowTransformer
    {
        String[] transformHeader(String[] header);
        String[] transformRow(String[] row);
    }
}