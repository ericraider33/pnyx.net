using pnyx.net.api;
using pnyx.net.impl.csv;
using pnyx.net.processors;
using pnyx.net.util;

namespace pnyx.net.fluent
{
    public class CsvModifer : IStreamFactoryModifier
    {
        public bool strict;                
        
        public IProcessor buildProcessor(StreamInformation streamInformation, IStreamFactory streamFactory)
        {
            CsvStreamToRowProcessor result = new CsvStreamToRowProcessor();
            result.setStrict(strict);
            result.setSource(streamInformation, streamFactory);
            return result;
        }
    }
}