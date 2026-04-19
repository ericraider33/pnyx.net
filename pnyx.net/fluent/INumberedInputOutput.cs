using System;

namespace pnyx.net.fluent;

public interface INumberedInputOutput
{
    string? getImpliedInputFileName();
    string? getImpliedOutputFileName();
    string? getFileName(int argNumber);            // 1 is min value
    bool verifyAllUsed();
}