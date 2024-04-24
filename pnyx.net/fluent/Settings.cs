using System;
using System.IO;
using System.Text;
using pnyx.net.impl;
using pnyx.net.impl.csv;
using pnyx.net.util;

namespace pnyx.net.fluent;

public class Settings : ICloneable
{
    public const char DEFAULT_CSV_DELIMITER = ',';
    public const char DEFAULT_CSV_ESCAPE_CHAR = '"';
    public static Encoding DEFAULT_ENCODING { get; private set; }

    static Settings()
    {
        DEFAULT_ENCODING = Encoding.ASCII;            
    }
        
    public String tempDirectory { get; set; }
    public int bufferLines { get; set; }
    public Encoding defaultEncoding { get; set; }
    public Encoding outputEncoding { get; set; }
    public bool detectEncodingFromByteOrderMarks { get; set; }
    public bool outputByteOrderMarks { get; set; }
    public String defaultNewline { get; set; }  
    public String outputNewline { get; set; }  
    public bool backupRewrite { get; set; }
    public bool processOnDispose { get; set; }
    public bool stdIoDefault { get; set; }

    private readonly CsvSettings csv;
    public char csvDelimiter
    {
        get => csv.delimiter;
        set => csv.delimiter = value;
    }
    public char csvEscapeChar 
    {
        get => csv.escapeChar;
        set => csv.escapeChar = value;
    }
    public TrimStyleEnum trimStyle
    {
        get => csv.trimStyle;
        set => csv.trimStyle = value;
    }

    public Settings()
    {
        tempDirectory = Path.GetTempPath();
        bufferLines = 10000;
        defaultEncoding = DEFAULT_ENCODING;
        outputEncoding = null;
        detectEncodingFromByteOrderMarks = true;
        outputByteOrderMarks = true;
        defaultNewline = Environment.NewLine;
        outputNewline = null;
        backupRewrite = true;
        processOnDispose = true;
        stdIoDefault = false;
        csv = new CsvSettings();
    }

    public StreamInformation buildStreamInformation()
    {
        return new StreamInformation(this);
    }

    public CsvSettings buildCsvSettings()
    {
        return (CsvSettings) csv.Clone();
    }
        
    public Object Clone()
    {
        return MemberwiseClone();
    }
}