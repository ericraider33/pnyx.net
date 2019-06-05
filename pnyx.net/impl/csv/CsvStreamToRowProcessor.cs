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
    public class CsvStreamToRowProcessor : IRowSource, IDisposable
    {
        public CsvRowConverter rowConverter { get; }
        public CsvSettings settings  { get; }
        public bool hasHeader { get; set; }
        public StreamReader reader { get; protected set; }
        public IRowProcessor rowProcessor { get; private set; }
        public StreamInformation streamInformation { get; protected set; }        
        public IStreamFactory streamFactory { get; protected set; }
        
        private readonly StringBuilder stringBuilder = new StringBuilder();
        private bool endOfFile;

        public CsvStreamToRowProcessor(CsvSettings settings = null)
        {
            this.settings = settings ?? new CsvSettings();
            rowConverter = new CsvRowConverter(this.settings);
        }

        public IRowConverter getRowConverter()
        {
            return rowConverter;
        }

        public virtual void process()
        {
            Stream stream = streamFactory.openStream();
            reader = new StreamReader(stream, streamInformation.defaultEncoding, streamInformation.detectEncodingFromByteOrderMarks);
            
            endOfFile = false;
            List<String> current;
            while ((current = readRow(streamInformation.lineNumber)) != null && streamInformation.active)
            {
                streamInformation.lineNumber++;
                if (streamInformation.lineNumber == 1 && hasHeader)
                    rowProcessor.rowHeader(current);
                else
                    rowProcessor.processRow(current);
            }

            if (!streamInformation.active)
                streamInformation.endsWithNewLine = current != null;

            rowProcessor.endOfFile();
            streamFactory.closeStream();
        }
                
        private enum  CsvState { StartOfLine, Quoted, Data, Seeking }
        
        protected virtual List<String> readRow(int rowNumber)
        {
            List<String> row = new List<String>();
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
                        if (num == '\n')
                        {
                            updateStreamInformation(rowNumber, "\n");
                            if (state != CsvState.StartOfLine)
                                row.Add(stringBuilder.ToString());                                    
                            return row;
                        }
                        else if (num == '\r')
                        {
                            if (reader.Peek() == '\n')
                            {
                                reader.Read();            // consumes both \r\n
                                updateStreamInformation(rowNumber, "\r\n");                            
                            }
                            else
                                updateStreamInformation(rowNumber, "\r");
                        
                            if (state != CsvState.StartOfLine)
                                row.Add(stringBuilder.ToString());                                    
                            return row;
                        }
                        else if (num == settings.delimiter)
                        {
                            row.Add(stringBuilder.ToString());
                            stringBuilder.Clear();
                            state = CsvState.Seeking;                                
                        }
                        else if (num == settings.escapeChar)
                        {
                            if (state == CsvState.Data)
                            {
                                if (settings.allowStrayQuotes)
                                    stringBuilder.Append(settings.escapeChar);
                                else 
                                    throw new IllegalStateException(String.Format("Line {0} contains a quote that isn't wrapped with quotes", rowNumber+1)); 
                            }
                            else
                                state = CsvState.Quoted;
                        }
                        else
                        {
                            state = CsvState.Data;
                            stringBuilder.Append((char)num);
                        }
                        break;
                    }

                    case CsvState.Quoted:
                    {
                        if (num == settings.escapeChar)
                        {
                            int next = reader.Peek();
                            if (next == settings.escapeChar)
                            {
                                reader.Read();            // consumes second quote
                                stringBuilder.Append(settings.escapeChar);
                            }
                            else if (next == settings.delimiter)
                            {
                                row.Add(stringBuilder.ToString());
                                stringBuilder.Clear();
                                reader.Read();            // consumes comma
                                state = CsvState.Seeking;                                
                            }
                            else if (next == '\n' || next == '\r' || next == -1)
                            {
                                state = CsvState.Data;
                            }
                            else
                            {
                                if (settings.allowTextAfterClosingQuote)
                                    state = CsvState.Data;
                                else
                                    throw new IllegalStateException(String.Format("Line {0} contains an unexpected character {1} after quote", rowNumber+1, (char)next));
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
                    if (!settings.terminateQuoteOnEndOfFile)
                        throw new IllegalStateException("File ends with open quotes");
                    break;
            }
            
            row.Add(stringBuilder.ToString());                                    
            return row;
        }

        private void updateStreamInformation(int lineNumber, String newLine)
        {
            if (lineNumber > 0)
                return;

            streamInformation.updateStreamNewLine(newLine);
            streamInformation.updateStreamEncoding(reader.CurrentEncoding);
        }
        
        public void Dispose()
        {
            if (reader != null)                    
                reader.Dispose();
            
            reader = null;

            IDisposable sfDisposable = (IDisposable) streamFactory; 
            if (sfDisposable != null)
                sfDisposable.Dispose();

            streamFactory = null;
        }

        public void setSource(StreamInformation streamInformation, IStreamFactory streamFactory)
        {
            this.streamInformation = streamInformation;
            this.streamFactory = streamFactory;
        }

        public void setNextRowProcessor(IRowProcessor next)
        {
            rowProcessor = next;
        }
    }
}