using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pnyx.net.processors;

public interface IRowProcessor : IPart
{
    Task rowHeader(List<String> header);
    Task processRow(List<String?> row);
    Task endOfFile();
}