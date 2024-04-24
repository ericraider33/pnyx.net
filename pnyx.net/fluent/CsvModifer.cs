using pnyx.net.api;
using pnyx.net.impl.csv;
using pnyx.net.processors;
using pnyx.net.util;

namespace pnyx.net.fluent;

public class CsvModifer : IStreamFactoryModifier
{
    public bool hasHeader { get; }
    public CsvSettings settings { get; }

    public CsvModifer(CsvSettings settings, bool hasHeader)
    {
        this.settings = settings;
        this.hasHeader = hasHeader;
    }

    public IProcessor buildProcessor(StreamInformation streamInformation, IStreamFactory streamFactory)
    {
        CsvStreamToRowProcessor result = new CsvStreamToRowProcessor(settings);
        result.hasHeader = hasHeader;
        result.setSource(streamInformation, streamFactory);
        return result;
    }
}