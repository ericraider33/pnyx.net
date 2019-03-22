using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using pnyx.net.errors;
using pnyx.net.fluent;
using pnyx.net.util;

namespace pnyx.net.impl.csv
{
    public class CsvWriter : RowToCsvStream
    {
        public CsvWriter(Stream stream, Encoding defaultEncoding = null) : 
            base(fromDefaultEncoding(defaultEncoding), stream)
        {
            writer = new StreamWriter(stream, streamInformation.getOutputEncoding());
        }

        private static StreamInformation fromDefaultEncoding(Encoding defaultEncoding = null)
        {
            Settings settings = SettingsHome.settingsFactory.buildSettings();            
            settings.outputEncoding = defaultEncoding;
            
            return new StreamInformation(settings);
        }        
        
        public override void rowHeader(List<String> header)
        {
            throw new IllegalStateException("Use writeRow method instead");            
        }        

        public override void processRow(List<String> row)
        {
            throw new IllegalStateException("Use writeRow method instead");            
        }

        public override void endOfFile()
        {
            throw new IllegalStateException("Use writeRow method instead");            
        }

        public void writeRow(List<String> row)
        {
            writeRow_(row);
            writer.WriteLine();
        }
    }
}