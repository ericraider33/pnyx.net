using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pnyx.net.processors;

public interface INameValuePairProcessor : IPart
{
    Task processNameValuePair(IDictionary<string, Object?> record);
    Task endOfFile();
}