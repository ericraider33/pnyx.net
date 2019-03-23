using System;
using System.IO;
using System.Text;
using pnyx.net.util;

namespace pnyx.net.fluent
{
    public class Settings : ICloneable
    {
        public String tempDirectory { get; set; }
        public int bufferLines { get; set; }
        public Encoding defaultEncoding { get; set; }
        public Encoding outputEncoding { get; set; }
        public bool detectEncodingFromByteOrderMarks { get; set; }
        public bool outputByteOrderMarks { get; set; }
        public String defaultNewline { get; set; }  
        public String outputNewline { get; set; }  
        public bool backupRewrite { get; set; }
        public bool processOnDispose { get; set; }
        public bool stdIoDefault { get; set; }

        public Settings()
        {
            tempDirectory = Path.GetTempPath();
            bufferLines = 10000;
            defaultEncoding = Encoding.ASCII;
            outputEncoding = null;
            detectEncodingFromByteOrderMarks = true;
            outputByteOrderMarks = true;
            defaultNewline = Environment.NewLine;
            outputNewline = null;
            backupRewrite = true;
            processOnDispose = true;
            stdIoDefault = false;
        }

        public StreamInformation buildStreamInformation()
        {
            return new StreamInformation(this);
        }

        public Object Clone()
        {
            return MemberwiseClone();
        }
    }
}