using System;

namespace pnyx.net.api
{
    public interface IRowTransformer
    {
        String[] transformRow(String[] values);
    }
}