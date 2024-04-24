using System;

namespace pnyx.net.impl.csv;

public class CsvSettings : ICloneable
{
    public char delimiter { get; set; }
    public char escapeChar { get; set; }
    public char[] charsNeedEscape { get; set; }                  
    public bool strict { get; set; }
    public TrimStyleEnum trimStyle { get; set; }
        
    public bool allowStrayQuotes => !strict;
    public bool allowTextAfterClosingQuote => !strict;
    public bool terminateQuoteOnEndOfFile => !strict;
    public bool allowUnquotedNewlines => !strict;
                
    public CsvSettings(bool strict = true, 
        char? delimiter = null, 
        char? escapeChar = null,
        char[] charsNeedEscape = null,
        TrimStyleEnum trimStyle = TrimStyleEnum.None
        )
    {
        this.strict = strict;
        this.delimiter = delimiter ?? CsvUtil.DEFAULT_DELIMITER;
        this.escapeChar = escapeChar ?? CsvUtil.DEFAULT_ESCAPE_CHAR;
        this.charsNeedEscape = CsvUtil.createCharsNeedEscape(this.delimiter, this.escapeChar, charsNeedEscape);
        this.trimStyle = trimStyle;
    }
        
    public CsvSettings setDefaults(
        bool? strict = null, 
        char? delimiter = null, 
        char? escapeChar = null,
        char[] charsNeedEscape = null,
        TrimStyleEnum? trimStyle = null
    )
    {
        if (strict.HasValue) this.strict = strict.Value;
        if (delimiter.HasValue) this.delimiter = delimiter.Value;
        if (escapeChar.HasValue) this.escapeChar = escapeChar.Value;
        if (trimStyle.HasValue) this.trimStyle = trimStyle.Value;

        if (charsNeedEscape != null)
            this.charsNeedEscape = CsvUtil.createCharsNeedEscape(this.delimiter, this.escapeChar, charsNeedEscape);
        else if (delimiter.HasValue || escapeChar.HasValue)
            this.charsNeedEscape = CsvUtil.createCharsNeedEscape(this.delimiter, this.escapeChar, this.charsNeedEscape);

        return this;
    }      
                
    public Object Clone()
    {
        return MemberwiseClone();
    }        
}