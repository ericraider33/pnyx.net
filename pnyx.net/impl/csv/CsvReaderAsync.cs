using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using pnyx.net.errors;
using pnyx.net.fluent;
using pnyx.net.util;

namespace pnyx.net.impl.csv
{
    public class CsvReaderAsync : IDisposable
    {
        public StreamReader reader { get; private set; }
        public StreamInformation streamInformation { get; private set; }        
        public CsvSettings csvSettings { get; }
        
        private readonly StringBuilder stringBuilder = new StringBuilder();
        private bool endOfFile;

        private char[] buffer = new char[8096];
        private int bufferSize;
        private int bufferPosition;
        
        private enum CsvState { StartOfLine, Quoted, Data, Seeking }
        
        public CsvReaderAsync(Stream stream, Encoding defaultEncoding = null, CsvSettings csvSettings = null) 
        {        
            this.csvSettings = csvSettings ?? new CsvSettings();
            
            Settings settings = SettingsHome.settingsFactory.buildSettings();
            settings.defaultEncoding = defaultEncoding ?? settings.defaultEncoding;
            
            streamInformation = new StreamInformation(settings);
                
            reader = new StreamReader(stream, streamInformation.defaultEncoding, streamInformation.detectEncodingFromByteOrderMarks);
        }
        
        private async Task<int> readChar_(bool peek = false)
        {
            if (bufferPosition >= bufferSize)
            {
                if (endOfFile)
                    return -1;
                
                bufferSize = await reader.ReadAsync(buffer, 0, buffer.Length);
                bufferPosition = 0;
                
                if (bufferSize == 0)
                {
                    endOfFile = true;
                    return -1;
                }
            }
            
            char result = buffer[bufferPosition];
            if (!peek)
                bufferPosition++;
            return result;
        }
        
        private Task<int> readChar()
        {
            return readChar_();
        }

        private Task<int> peekChar()
        {
            return readChar_(peek: true);
        }
        
        public async Task<List<String>> readRow()
        {
            List<String> result = await readRow(streamInformation.lineNumber);
            streamInformation.lineNumber++;
            return result;
        }
        
        public async Task<List<String>> readRow(int rowNumber)
        {
            List<String> row = new List<String>();
            stringBuilder.Clear();
            if (endOfFile)
                return null;            
            
            int num;
            CsvState state = CsvState.StartOfLine;
            while ((num = await readChar()) != -1)
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
                            if (await peekChar() == '\n')
                            {
                                await readChar();            // consumes both \r\n
                                updateStreamInformation(rowNumber, "\r\n");                            
                            }
                            else
                                updateStreamInformation(rowNumber, "\r");
                        
                            if (state != CsvState.StartOfLine)
                                row.Add(stringBuilder.ToString());                                    
                            return row;
                        }
                        else if (num == csvSettings.delimiter)
                        {
                            row.Add(stringBuilder.ToString());
                            stringBuilder.Clear();
                            state = CsvState.Seeking;                                
                        }
                        else if (num == csvSettings.escapeChar)
                        {
                            if (state == CsvState.Data)
                            {
                                if (csvSettings.allowStrayQuotes)
                                    stringBuilder.Append(csvSettings.escapeChar);
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
                        if (num == csvSettings.escapeChar)
                        {
                            int next = await peekChar();
                            if (next == csvSettings.escapeChar)
                            {
                                await readChar();            // consumes second quote
                                stringBuilder.Append(csvSettings.escapeChar);
                            }
                            else if (next == csvSettings.delimiter)
                            {
                                row.Add(stringBuilder.ToString());
                                stringBuilder.Clear();
                                await readChar();            // consumes comma
                                state = CsvState.Seeking;                                
                            }
                            else if (next == '\n' || next == '\r' || next == -1)
                            {
                                state = CsvState.Data;
                            }
                            else
                            {
                                if (csvSettings.allowTextAfterClosingQuote)
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
                    if (!csvSettings.terminateQuoteOnEndOfFile)
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
        }
    }
}