using System;
using System.Text;

namespace pnyx.net.util
{
    public class StreamInformation
    {
        public Encoding encoding;
        public String newLine;
        public bool endsWithNewLine;

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
    }
}