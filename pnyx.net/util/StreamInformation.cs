using System;
using System.Text;
using pnyx.net.fluent;

namespace pnyx.net.util
{
    public class StreamInformation
    {
        public const String NEWLINE_UNIX = "\n";
        public const String NEWLINE_WINDOWS = "\r\n";
        public const String LINEFEED = "\r";
        
        public Encoding streamEncoding { get; private set; }        // set from input stream
        
        public String streamNewLine { get; private set; }        // set from input stream
        public bool endsWithNewLine;

        public int lineNumber = 0;
        public bool active = true;      
        
        private readonly Settings settings;
        public bool detectEncodingFromByteOrderMarks => settings.detectEncodingFromByteOrderMarks;
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
                case NEWLINE_UNIX: return NewLineEnum.Unix;
                case NEWLINE_WINDOWS: return NewLineEnum.Windows;
                case LINEFEED: return NewLineEnum.LineFeed;
                default: return NewLineEnum.None;
            }
        }

        public static String newlineString(NewLineEnum x)
        {
            switch (x)
            {
                default:
                case NewLineEnum.None: return null;
                case NewLineEnum.Unix: return NEWLINE_UNIX;
                case NewLineEnum.Windows: return NEWLINE_WINDOWS;
                case NewLineEnum.LineFeed: return LINEFEED;
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
            Encoding result = settings.defaultEncoding;
            if (settings.outputEncoding != null)
                result = settings.outputEncoding;
            else if (streamEncoding != null)
                result = streamEncoding;
            else
                result = settings.defaultEncoding;

            return new EncodingProxy(result, settings);
        }
    }
}