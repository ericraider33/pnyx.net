using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using pnyx.net.errors;
using pnyx.net.processors.sources;
using pnyx.net.util;

namespace pnyx.net.impl.csv
{
    public class CsvReader : CsvStreamToRowProcessor
    {        
        public CsvReader(Stream stream, Encoding defaultEncoding = null)
        {            
            streamInformation = new StreamInformation();
            if (defaultEncoding != null)
                streamInformation.defaultEncoding = defaultEncoding;            
                
            streamFactory = new GenericStreamFactory(stream);            
            reader = new StreamReader(stream, streamInformation.defaultEncoding, streamInformation.detectEncodingFromByteOrderMarks);            
        }

        public bool EndOfStream => reader.EndOfStream;

        public override void process()
        {
            throw new IllegalStateException("Use readRow method instead");
        }

        public List<String> readRow()
        {
            List<String> result = readRow(streamInformation.lineNumber);
            streamInformation.lineNumber++;
            return result;
        }
    }    
}