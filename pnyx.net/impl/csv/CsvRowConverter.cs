using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using pnyx.net.api;
using pnyx.net.processors;
using pnyx.net.util;

namespace pnyx.net.impl.csv;

public class CsvRowConverter : IRowConverter
{
    public CsvSettings settings { get; set;  }
    private readonly StringBuilder stringBuilder = new ();

    public CsvRowConverter()
    {
        settings = new CsvSettings();
    }

    public CsvRowConverter(CsvSettings settings)
    {
        this.settings = settings;
    }
        
    public List<String?> lineToRow(String source)
    {
        return CsvUtil.parseRow(source, settings.delimiter, settings.escapeChar,
            settings.allowStrayQuotes, settings.allowTextAfterClosingQuote, settings.terminateQuoteOnEndOfFile, settings.allowUnquotedNewlines,
            settings.trimStyle,
            stringBuilder
        ) ?? new List<string?>();
    }

    public String rowToLine(List<String?> source)
    {
        return CsvUtil.rowToString(source, settings.delimiter, settings.escapeChar, settings.charsNeedEscape) ?? "";
    }

    public IRowProcessor buildRowDestination(StreamInformation streamInformation, Stream stream)
    {
        return new RowToCsvStream(streamInformation, stream, settings);
    }
}