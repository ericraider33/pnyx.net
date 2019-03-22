using System;
using System.Text;

namespace pnyx.net.util
{
    public class StreamInformation
    {
        public Encoding encoding;
        public Encoding defaultEncoding;
        public bool detectEncodingFromByteOrderMarks = true;
        
        public String newLine { get; private set; }
        public bool endsWithNewLine;
        public String defaultNewLine;
        public int lineNumber = 0;
        public bool active = true;        

        public StreamInformation(Encoding defaultEncoding = null, String defaultNewLine = null)
        {
            this.defaultEncoding = defaultEncoding ?? Encoding.ASCII;
            this.defaultNewLine = defaultNewLine ?? Environment.NewLine;
        }

        public String getNewline()
        {
            return newLine ?? defaultNewLine;
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

        public void setNewLine(NewLineEnum newLineValue)
        {
            newLine = newlineString(newLineValue);
        }

        public void setDefaultNewline(NewLineEnum newLineValue)
        {
            defaultNewLine = newlineString(newLineValue);
        }                

        public void updateNewLine(String newLineValue)
        {
            newLine = newLine ?? newLineValue;
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
        
        public Encoding getOutputEncoding()
        {
            return encoding ?? defaultEncoding;
        }
    }
}