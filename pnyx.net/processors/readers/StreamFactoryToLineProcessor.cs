using System.IO;
using System.Text;
using pnyx.net.api;
using pnyx.net.util;

namespace pnyx.net.processors.readers
{
    public class StreamFactoryToLineProcessor : StreamToLineProcessor, ILineSource
    {
        public IStreamFactory streamFactory { get; private set; }

        public StreamFactoryToLineProcessor(StreamInformation streamInformation, IStreamFactory streamFactory)
        {
            this.streamInformation = streamInformation;
            this.streamFactory = streamFactory;
        }

        public override void process()
        {
            Stream stream = streamFactory.openStream();
            reader = new StreamReader(stream, Encoding.ASCII, true);
            
            base.process();

            streamFactory.closeStream();
        }
    }
}