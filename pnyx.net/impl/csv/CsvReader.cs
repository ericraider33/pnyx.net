using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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
    public CsvReader
    (
        Stream stream, 
        Encoding? defaultEncoding = null, 
        CsvSettings? csvSettings = null
    ) : base(csvSettings)
    {        
        Settings streamSettings = SettingsHome.settingsFactory.buildSettings();
        streamSettings.defaultEncoding = defaultEncoding ?? streamSettings.defaultEncoding;
            
        streamInformation = new StreamInformation(streamSettings);
                
        streamFactory = new GenericStreamFactory(stream);            
        reader = new StreamReader(stream, streamInformation.defaultEncoding, streamInformation.detectEncodingFromByteOrderMarks);            
    }

    public override Task process()
    {
        throw new IllegalStateException("Use readRow method instead");
    }

    public async Task<List<String?>?> readRow()
    {
        List<String?>? result = await readRow(streamInformation!.lineNumber);
        if (result != null)
            streamInformation.lineNumber++;
        
        return result;
    }
}