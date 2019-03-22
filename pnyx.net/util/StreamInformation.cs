using System;
using System.Text;
using pnyx.net.fluent;

namespace pnyx.net.util
{
    public class StreamInformation
    {
        public Encoding streamEncoding { get; private set; }        // set from input stream
        public bool detectEncodingFromByteOrderMarks = true;
        
        public String streamNewLine { get; private set; }        // set from input stream
        public bool endsWithNewLine;

        public int lineNumber = 0;
        public bool active = true;      
        
        private readonly Settings settings;
        public Encoding defaultEncoding => settings.defaultEncoding;

        public StreamInformation(Settings settings)
        {
            this.settings = settings;
        }        

        public void updateStreamNewLine(String newLineValue)
        {
            if (streamNewLine != null)
                return;
            
            streamNewLine = newLineValue;
        }

        public String getOutputNewline()
        {
            if (settings.outputNewline != null)
                return settings.outputNewline;
            
            if (streamNewLine != null)
                return streamNewLine;
            
            return settings.defaultNewline;
        }
        
        public NewLineEnum retrieveStreamNewLineEnum()
        {
            if (streamNewLine == null)
                return NewLineEnum.None;

            switch (streamNewLine)
            {
                case "\n": return NewLineEnum.Unix;
                case "\r\n": return NewLineEnum.Windows;
                case "\r" : return NewLineEnum.LineFeed;
                default: return NewLineEnum.None;
            }
        }

        public static String newlineString(NewLineEnum x)
        {
            switch (x)
            {
                default:
                case NewLineEnum.None: return null;
                case NewLineEnum.Unix: return "\n";
                case NewLineEnum.Windows: return "\r\n";
                case NewLineEnum.LineFeed: return "\r";
                case NewLineEnum.Native: return Environment.NewLine;
            }
        }

        public void updateStreamEncoding(Encoding encoding)
        {
            if (streamEncoding != null)
                return;
            
            streamEncoding = encoding;
        }

        public Encoding getOutputEncoding()
        {
            if (settings.outputEncoding != null)
                return settings.outputEncoding;
                
            if (streamEncoding != null)
                return streamEncoding;
            
            return settings.defaultEncoding;
        }
    }
}