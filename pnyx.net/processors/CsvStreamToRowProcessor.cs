using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using pnyx.net.api;
using pnyx.net.errors;
using pnyx.net.transforms;
using pnyx.net.util;

namespace pnyx.net.processors
{
    public class CsvStreamToRowProcessor : IRowSource, IDisposable
    {
        public CsvRowConverter rowConverter { get; private set; }
        public StreamReader reader { get; private set; }
        public IRowProcessor rowProcessor { get; private set; }
        public StreamInformation streamInformation { get; private set; }
        
        private readonly StringBuilder stringBuilder = new StringBuilder();
        private readonly List<String> row = new List<String>();
        private bool endOfFile;


        public CsvStreamToRowProcessor()
        {
            rowConverter = new CsvRowConverter();
        }

        public IRowConverter getRowConverter()
        {
            return rowConverter;
        }
        
        public void setStrict(bool strict)
        {
            rowConverter.setStrict(strict);
        }

        public void process()
        {
            endOfFile = false;
            int rowNumber = 0;
            String[] current;
            while ((current = readRow(rowNumber))!= null)
            {
                rowNumber++;
                rowProcessor.processRow(current);
            }

            rowProcessor.endOfFile();
        }
                
        private enum  CsvState { StartOfLine, Quoted, Data, Seeking }
        
        protected virtual string[] readRow(int rowNumber)
        {
            row.Clear();
            stringBuilder.Clear();
            if (endOfFile)
                return null;            
            
            int num;
            CsvState state = CsvState.StartOfLine;
            while ((num = reader.Read()) != -1)
            {
                switch (state)
                {                    
                    case CsvState.Data:
                    case CsvState.StartOfLine:
                    case CsvState.Seeking:
                    {
                        switch (num)
                        {
                            case 10:
                                updateStreamInformation(rowNumber, "\n");
                                if (state != CsvState.StartOfLine)
                                    row.Add(stringBuilder.ToString());                                    
                                return row.ToArray();

                            case 13:
                                if (reader.Peek() == 10)
                                {
                                    reader.Read();            // consumes both \r\n
                                    updateStreamInformation(rowNumber, "\r\n");                            
                                }
                                else
                                    updateStreamInformation(rowNumber, "\r");
                        
                                if (state != CsvState.StartOfLine)
                                    row.Add(stringBuilder.ToString());                                    
                                return row.ToArray();
                            
                            case ',':
                                row.Add(stringBuilder.ToString());
                                stringBuilder.Clear();
                                state = CsvState.Seeking;                                
                                break;
                            
                            case '"':
                                if (state == CsvState.Data)
                                {
                                    if (rowConverter.allowStrayQuotes)
                                        stringBuilder.Append('"');
                                    else 
                                        throw new IllegalStateException(String.Format("Line {0} contains a quote that isn't wrapped with quotes", rowNumber+1)); 
                                }
                                else
                                    state = CsvState.Quoted;
                                break;
                        
                            default:
                                state = CsvState.Data;
                                stringBuilder.Append((char)num);
                                break;
                        }
                        break;
                    }

                    case CsvState.Quoted:
                    {
                        if (num == '"')
                        {
                            int next = reader.Peek();
                            switch (next)
                            {
                                case '"': 
                                    reader.Read();            // consumes second quote
                                    stringBuilder.Append('"');
                                    break;
                                
                                case ',':
                                    row.Add(stringBuilder.ToString());
                                    stringBuilder.Clear();
                                    reader.Read();            // consumes comma
                                    state = CsvState.Seeking;                                
                                    break;
                                
                                case 10:
                                case 13:
                                case -1:
                                    state = CsvState.Data;
                                    break;
                                
                                default:
                                    if (rowConverter.allowTextAfterClosingQuote)
                                        state = CsvState.Data;
                                    else
                                        throw new IllegalStateException(String.Format("Line {0} contains an unexpected character {1} after quote", rowNumber+1, (char)next));
                                    break;
                            }
                        }
                        else
                        {
                            stringBuilder.Append((char)num);
                        }
                        break;
                    }
                }                
            }            

            endOfFile = true;
            updateStreamInformation(rowNumber, null);

            switch (state)
            {
                case CsvState.StartOfLine:
                    if (rowNumber > 0)
                        streamInformation.endsWithNewLine = true;
                    return null;
                
                case CsvState.Quoted:
                    if (!rowConverter.terminateQuoteOnEndOfFile)
                        throw new IllegalStateException("File ends with open quotes");
                    break;
            }
            
            row.Add(stringBuilder.ToString());                                    
            return row.ToArray();
        }

        private void updateStreamInformation(int lineNumber, String newLine)
        {
            if (lineNumber > 0)
                return;

            streamInformation.newLine = newLine;
            streamInformation.encoding = reader.CurrentEncoding;
        }
        
        public void Dispose()
        {
            if (reader != null)                    
                reader.Dispose();
            
            reader = null;
        }

        public void setSource(StreamInformation streamInformation, Stream stream, IRowProcessor rowProcessor)
        {
            this.streamInformation = streamInformation;
            reader = new StreamReader(stream, streamInformation.defaultEncoding, true);
            this.rowProcessor = rowProcessor;
        }
    }
}