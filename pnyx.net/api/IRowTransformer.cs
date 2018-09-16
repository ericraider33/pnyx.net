using System;

namespace pnyx.net.api
{
    public interface IColumnTransformer
    {
        String[] transformRow(String[] values);
    }
}