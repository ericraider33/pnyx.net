using System;
using System.Text;

namespace pnyx.net.util
{
    public class StreamInformation
    {
        public Encoding encoding;
        public String newLine;
        public bool endsWithNewLine;
        public Encoding defaultEncoding;
        public String defaultNewline;

        public StreamInformation()
        {
            defaultEncoding = Encoding.ASCII;
            defaultNewline = Environment.NewLine;
        }

        public String getNewline()
        {
            return newLine ?? defaultNewline;
        }
        
        public NewLineEnum retrieveNewLineEnum()
        {
            if (newLine == null)
                return NewLineEnum.None;

            switch (newLine)
            {
                case "\n": return NewLineEnum.Unix;
                case "\r\n": return NewLineEnum.Windows;
                case "\r" : return NewLineEnum.LineFeed;
                default: return NewLineEnum.None;
            }
        }

        public void setDefaultNewline(NewLineEnum defaultValue)
        {
            defaultNewline = newlineString(defaultValue);
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
            }
        }
    }
}