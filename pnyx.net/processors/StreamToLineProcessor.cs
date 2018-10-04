using System;
using System.IO;
using System.Text;
using pnyx.net.util;

namespace pnyx.net.processors
{
    public class StreamToLineProcessor : IProcessor, IDisposable
    {
        public StreamReader reader { get; private set; }
        public ILineProcessor lineProcessor;
        public readonly StreamInformation streamInformation;
        
        private readonly StringBuilder stringBuilder = new StringBuilder();
        private bool endOfFile;
        
        public StreamToLineProcessor(StreamInformation streamInformation, Stream stream, ILineProcessor lineProcessor)
        {
            this.streamInformation = streamInformation;
            reader = new StreamReader(stream, Encoding.ASCII, true);
            this.lineProcessor = lineProcessor;
        }

        public void process()
        {
            endOfFile = false;
            int lineNumber = 0;
            String line;
            while ((line = readLine(lineNumber))!= null)
            {
                lineNumber++;
                lineProcessor.process(line);
            }

            lineProcessor.endOfFile();
        }
        
        protected virtual string readLine(int lineNumber)
        {
            stringBuilder.Clear();
            if (endOfFile)
                return null;            
            
            int num;
            while ((num = reader.Read()) != -1)
            {
                switch (num)
                {
                    case 10:
                        updateStreamInformation(lineNumber, "\n");                            
                        return stringBuilder.ToString();

                    case 13:
                        if (reader.Peek() == 10)
                        {
                            reader.Read();            // consumes both \r\n
                            updateStreamInformation(lineNumber, "\r\n");                            
                        }
                        else
                            updateStreamInformation(lineNumber, "\r");
                        
                        return stringBuilder.ToString();
                        
                    default:
                        stringBuilder.Append((char)num);
                        continue;
                }
            }            

            endOfFile = true;
            updateStreamInformation(lineNumber, null);

            if (stringBuilder.Length > 0)
                return stringBuilder.ToString();

            // Sets flag because an empty string
            if (lineNumber > 0)
                streamInformation.endsWithNewLine = true;
            
            return null;
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
    }
}