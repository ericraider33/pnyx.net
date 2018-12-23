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
        public String defaultNewline { get; set; }  
        public bool backupRewrite { get; set; }
        public bool processOnDispose { get; set; }
        public bool stdIoDefault { get; set; }

        public Settings()
        {
            tempDirectory = Path.GetTempPath();
            bufferLines = 10000;
            defaultEncoding = Encoding.ASCII;
            defaultNewline = Environment.NewLine;
            backupRewrite = true;
            processOnDispose = true;
            stdIoDefault = false;
        }

        public StreamInformation buildStreamInformation()
        {
            return new StreamInformation(defaultEncoding, defaultNewline);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}