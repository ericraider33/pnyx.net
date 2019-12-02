using System;

namespace pnyx.net.fluent
{
    public interface INumberedInputOutput
    {
        String getImpliedInputFileName();
        String getImpliedOutputFileName();
        String getFileName(int argNumber);            // 1 is min value
    }
}