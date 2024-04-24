using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using pnyx.net.errors;
using pnyx.net.fluent;
using pnyx.net.processors.sources;
using pnyx.net.util;

namespace pnyx.net.impl.csv;

/// <summary>
/// A utility class for reading CSVs without building a pnyx pipeline. Use this class for
/// custom/generic handling of CSV data where pipelines are too inflexible. This class exports the powerful built-in
/// CSV parser without the need of using the pnyx api. 
/// </summary>
public class CsvReader : CsvStreamToRowProcessor
{        
    public CsvReader(Stream stream, Encoding defaultEncoding = null, CsvSettings csvSettings = null) : 
        base(csvSettings)
    {        
        Settings settings = SettingsHome.settingsFactory.buildSettings();
        settings.defaultEncoding = defaultEncoding ?? settings.defaultEncoding;
            
        streamInformation = new StreamInformation(settings);
                
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