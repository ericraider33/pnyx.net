using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using pnyx.net.errors;
using pnyx.net.fluent;

namespace pnyx.net.impl.csv
{
    public static class CsvUtil
    {
        public const char DEFAULT_DELIMITER = Settings.DEFAULT_CSV_DELIMITER;
        public const char DEFAULT_ESCAPE_CHAR = Settings.DEFAULT_CSV_ESCAPE_CHAR;

        public static char[] createCharsNeedEscape(
            char delimiter,
            char escapeChar,
            char[] charsNeedEscape = null
            )
        {
            if (charsNeedEscape != null &&
                charsNeedEscape.Contains(delimiter) &&
                charsNeedEscape.Contains(escapeChar))
                return charsNeedEscape;

            HashSet<char> toBuild;
            if (charsNeedEscape == null)
                toBuild = new HashSet<char> { '\n', '\r', '\t', ' ' };
            else
                toBuild = new HashSet<char>(charsNeedEscape);

            toBuild.Add(delimiter);
            toBuild.Add(escapeChar);

            return toBuild.ToArray();
        }
        
        public static void writeRowWithDefaults(
            TextWriter writer,
            IEnumerable<String> row,
            char? delimiter = null,
            char? escapeChar = null,
            char[] charsNeedEscape = null
            )
        {
            delimiter = delimiter ?? DEFAULT_DELIMITER;
            escapeChar = escapeChar ?? DEFAULT_ESCAPE_CHAR;
            charsNeedEscape = createCharsNeedEscape(delimiter.Value, escapeChar.Value, charsNeedEscape);
          
            writeRow(writer, row, delimiter.Value, escapeChar.Value, charsNeedEscape);
        }   
        
        public static void writeRow(
            TextWriter writer,
            IEnumerable<String> row,
            char delimiter,
            char escapeChar,
            char[] charsNeedEscape            
            )
        {
            bool first = true;
            foreach (String val in row)
            {
                if (!first)
                    writer.Write(delimiter);
                first = false;

                if (val == null)
                    continue;

                // Checks if escape needed
                if (val.IndexOfAny(charsNeedEscape) >= 0)
                {
                    writer.Write(escapeChar);
                    foreach (char c in val)
                    {
                        if (c == escapeChar)
                            writer.Write(escapeChar);

                        writer.Write(c);
                    }
                    writer.Write(escapeChar);
                }
                else
                    writer.Write(val);
            }
        }
                
        public static String rowToStringWithDefaults(
            IEnumerable<String> source,
            char? delimiter = null,
            char? escapeChar = null,
            char[] charsNeedEscape = null
        )
        {
            delimiter = delimiter ?? DEFAULT_DELIMITER;
            escapeChar = escapeChar ?? DEFAULT_ESCAPE_CHAR;
            charsNeedEscape = createCharsNeedEscape(delimiter.Value, escapeChar.Value, charsNeedEscape);
          
            return rowToString(source, delimiter.Value, escapeChar.Value, charsNeedEscape);
        }   
        
        public static String rowToString(IEnumerable<String> source,
            char delimiter,
            char escapeChar,
            char[] charsNeedEscape            
            )
        {
            if (source == null)
                return null;
            
            StringBuilder result = new StringBuilder();
            bool first = true;
            foreach (String val in source)
            {
                if (!first)
                    result.Append(delimiter);
                first = false;

                if (val == null)
                    continue;

                if (val.IndexOfAny(charsNeedEscape) >= 0)
                {
                    result.Append(escapeChar);
                    foreach (char c in val)
                    {
                        if (c == escapeChar)
                            result.Append(escapeChar);

                        result.Append(c);
                    }
                    result.Append(escapeChar);
                }
                else
                    result.Append(val);
            }

            return result.ToString();
        }        
        
        private enum CsvState { StartOfLine, Quoted, Data, Seeking }

        public static List<String> parseRowWithDefaults(String source,
            char? delimiter = null,
            char? escapeChar = null,
            bool strictMode = false
        )
        {
            return parseRow(source,
                delimiter ?? DEFAULT_DELIMITER,
                escapeChar ?? DEFAULT_ESCAPE_CHAR,
                !strictMode,
                !strictMode,
                !strictMode,
                !strictMode,
                new StringBuilder()
                );
        }
        
        public static List<String> parseRow(String source,
            char delimiter,
            char escapeChar,
            bool allowStrayQuotes, 
            bool allowTextAfterClosingQuote,
            bool terminateQuoteOnEndOfFile,
            bool allowUnquotedNewlines,
            StringBuilder stringBuilder = null 
            )
        {
            if (source == null)
                return null;

            if (stringBuilder == null)
                stringBuilder = new StringBuilder();
            else
                stringBuilder.Clear();
            
            List<String> row = new List<String>();
            
            CsvState state = CsvState.StartOfLine;
            for (int i = 0; i < source.Length; i++)
            {
                char c = source[i];
                switch (state)
                {                    
                    case CsvState.Data:
                    case CsvState.StartOfLine:
                    case CsvState.Seeking:
                    {
                        if (c == '\n' || c == '\r')
                        {
                            if (!allowUnquotedNewlines)                                    
                                throw new IllegalStateException(String.Format("Line contains a newline that isn't wrapped with quotes: {0}", source));

                            state = CsvState.Data;
                            stringBuilder.Append(c);
                        }
                        else if (c == delimiter)
                        {
                            row.Add(stringBuilder.ToString());
                            stringBuilder.Clear();
                            state = CsvState.Seeking;                                
                        }
                        else if (c == escapeChar)
                        {
                            if (state == CsvState.Data)
                            {
                                if (allowStrayQuotes)
                                    stringBuilder.Append(escapeChar);
                                else 
                                    throw new IllegalStateException(String.Format("Line contains a quote that isn't wrapped with quotes: {0}", source)); 
                            }
                            else
                                state = CsvState.Quoted;
                        }
                        else                        
                        {                            
                            
                            state = CsvState.Data;
                            stringBuilder.Append(c);
                        }
                        break;
                    }

                    case CsvState.Quoted:
                    {
                        if (c == escapeChar)
                        {
                            int next = i+1 < source.Length ? source[i+1] : -1;
                            if (next == escapeChar)
                            {
                                i++;                     // consumes second quote
                                stringBuilder.Append(escapeChar);
                            }
                            else if (next == delimiter)
                            {
                                row.Add(stringBuilder.ToString());
                                stringBuilder.Clear();
                                i++;                    // consumes comma
                                state = CsvState.Seeking;                                
                            }
                            else if (next == '\n' || next == '\r')
                            {
                                state = CsvState.Data;
                            }
                            else
                            {
                                if (allowTextAfterClosingQuote)
                                    state = CsvState.Data;
                                else
                                    throw new IllegalStateException(String.Format("Line contains an unexpected character {1} after quote: {0}", source, (char)next));
                            }
                        }
                        else
                        {
                            stringBuilder.Append(c);
                        }
                        break;
                    }
                }                
            }

            switch (state)
            {
                case CsvState.StartOfLine:
                    return null;
                
                case CsvState.Quoted:
                    if (!terminateQuoteOnEndOfFile)
                        throw new IllegalStateException("String ends with open quotes");
                    break;
            }
            
            row.Add(stringBuilder.ToString());                                    
            return row;            
        }
        
    }
}