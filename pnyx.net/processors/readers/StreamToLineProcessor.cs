using System;
using System.IO;
using System.Text;
using pnyx.net.util;

namespace pnyx.net.processors.readers
{
    public class StreamToLineProcessor : IProcessor, IDisposable, ILinePart
    {
        public StreamReader reader { get; protected set; }
        public StreamInformation streamInformation { get; protected set; }
        public ILineProcessor lineProcessor { get; protected set; }
        
        private readonly StringBuilder stringBuilder = new StringBuilder();
        private bool endOfFile;

        public StreamToLineProcessor()
        {            
        }
        
        public StreamToLineProcessor(StreamInformation streamInformation, Stream stream, ILineProcessor lineProcessor)
        {
            this.streamInformation = streamInformation;
            reader = new StreamReader(stream, Encoding.ASCII, true);
            this.lineProcessor = lineProcessor;
        }                

        public virtual void process()
        {
            endOfFile = false;
            String line;
            while ((line = readLine(streamInformation.lineNumber))!= null)
            {
                streamInformation.lineNumber++;
                lineProcessor.processLine(line);
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

        public void setNext(ILineProcessor next)
        {
            lineProcessor = next;
        }
    }
}