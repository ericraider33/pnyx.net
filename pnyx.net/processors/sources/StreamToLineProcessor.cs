using System;
using System.IO;
using System.Text;
using pnyx.net.api;
using pnyx.net.util;

namespace pnyx.net.processors.sources
{
    public class StreamToLineProcessor : IProcessor, IDisposable, ILinePart, ILineSource
    {
        public IStreamFactory streamFactory { get; protected set; }        
        public StreamReader reader { get; protected set; }
        public StreamInformation streamInformation { get; protected set; }
        public ILineProcessor lineProcessor { get; protected set; }
        
        private readonly StringBuilder stringBuilder = new StringBuilder();
        private bool endOfFile;

        public StreamToLineProcessor(StreamInformation streamInformation, IStreamFactory streamFactory)
        {
            this.streamInformation = streamInformation;
            this.streamFactory = streamFactory;
        }
                
        public StreamToLineProcessor(StreamInformation streamInformation, Stream stream)
        {
            this.streamInformation = streamInformation;
            streamFactory = new GenericStreamFactory(stream);
        }                

        public virtual void process()
        {
            Stream stream = streamFactory.openStream();
            reader = new StreamReader(stream, streamInformation.defaultEncoding, streamInformation.detectEncodingFromByteOrderMarks);
            
            endOfFile = false;
            String line;
            while ((line = readLine(streamInformation.lineNumber))!= null && streamInformation.active)
            {
                streamInformation.lineNumber++;
                lineProcessor.processLine(line);
            }

            if (!streamInformation.active)
                streamInformation.endsWithNewLine = line != null;

            lineProcessor.endOfFile();
            streamFactory.closeStream();            
        }
        
        protected virtual String readLine(int lineNumber)
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

            // Sets flag because an empty String
            if (lineNumber > 0)
                streamInformation.endsWithNewLine = true;
            
            return null;
        }

        private void updateStreamInformation(int lineNumber, String newLine)
        {
            if (lineNumber > 0)
                return;

            streamInformation.updateNewLine(newLine);
            streamInformation.encoding = reader.CurrentEncoding;
        }
        
        public void Dispose()
        {
            if (reader != null)                    
                reader.Dispose();            
            reader = null;
            
            if (streamFactory != null && streamFactory is IDisposable)
                ((IDisposable)streamFactory).Dispose();
            streamFactory = null;
        }

        public void setNextLineProcessor(ILineProcessor next)
        {
            lineProcessor = next;
        }
    }
}