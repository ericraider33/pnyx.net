using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using pnyx.net.api;
using pnyx.net.errors;
using pnyx.net.processors;
using pnyx.net.util;

namespace pnyx.net.impl.csv
{
    public class CsvRowConverter : IRowConverter
    {
        public bool allowStrayQuotes { get; set; }
        public bool allowTextAfterClosingQuote { get; set; }
        public bool terminateQuoteOnEndOfFile { get; set; }
        public bool allowUnquotedNewlines { get; set; }
        
        private readonly StringBuilder stringBuilder = new StringBuilder();
        
        public CsvRowConverter setStrict(bool strict)
        {
            allowStrayQuotes = !strict;
            allowTextAfterClosingQuote = !strict;
            terminateQuoteOnEndOfFile = !strict;
            allowUnquotedNewlines = !strict;

            return this;
        }

        private enum  CsvState { StartOfLine, Quoted, Data, Seeking }
        
        public List<String> lineToRow(String source)
        {
            if (source == null)
                return null;
            
            List<String> row = new List<String>();
            stringBuilder.Clear();
            
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
                        switch (c)
                        {
                            case '\n':
                            case '\r':
                                if (!allowUnquotedNewlines)                                    
                                    throw new IllegalStateException(String.Format("Line contains a newline that isn't wrapped with quotes: {0}", source));

                                state = CsvState.Data;
                                stringBuilder.Append(c);
                                break;
                            
                            case ',':
                                row.Add(stringBuilder.ToString());
                                stringBuilder.Clear();
                                state = CsvState.Seeking;                                
                                break;
                            
                            case '"':
                                if (state == CsvState.Data)
                                {
                                    if (allowStrayQuotes)
                                        stringBuilder.Append('"');
                                    else 
                                        throw new IllegalStateException(String.Format("Line contains a quote that isn't wrapped with quotes: {0}", source)); 
                                }
                                else
                                    state = CsvState.Quoted;
                                break;
                        
                            default:
                                state = CsvState.Data;
                                stringBuilder.Append(c);
                                break;
                        }
                        break;
                    }

                    case CsvState.Quoted:
                    {
                        if (c == '"')
                        {
                            int next = i+1 < source.Length ? source[i+1] : -1;
                            switch (next)
                            {
                                case '"': 
                                    i++;                     // consumes second quote
                                    stringBuilder.Append('"');
                                    break;
                                
                                case ',':
                                    row.Add(stringBuilder.ToString());
                                    stringBuilder.Clear();
                                    i++;                    // consumes comma
                                    state = CsvState.Seeking;                                
                                    break;
                                
                                case 10:
                                case 13:
                                case -1:
                                    state = CsvState.Data;
                                    break;
                                
                                default:
                                    if (allowTextAfterClosingQuote)
                                        state = CsvState.Data;
                                    else
                                        throw new IllegalStateException(String.Format("Line contains an unexpected character {1} after quote: {0}", source, (char)next));
                                    break;
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

        public String rowToLine(List<String> source)
        {
            if (source == null)
                return null;
            
            StringBuilder result = new StringBuilder();
            bool first = true;
            foreach (String val in source)
            {
                if (!first)
                    result.Append(",");
                first = false;

                if (val == null)
                    continue;

                if (val.IndexOfAny(new[] { ',', '"', '\n', '\r', '\t', ' ' }) >= 0)
                {
                    result.Append('"');
                    foreach (char c in val)
                    {
                        if (c == '"')
                            result.Append("\"\"");
                        else
                            result.Append(c);
                    }
                    result.Append('"');
                }
                else
                    result.Append(val);
            }

            return result.ToString();
        }

        public IRowProcessor buildRowDestination(StreamInformation streamInformation, Stream stream)
        {
            return new RowToCsvStream(streamInformation, stream);
        }
    }
}