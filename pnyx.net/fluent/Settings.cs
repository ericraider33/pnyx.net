using System;
using System.IO;
using System.Text;

namespace pnyx.net.fluent
{
    public class Settings
    {
        public String tempDirectory;
        public int bufferLines;
        public Encoding defaultEncoding;
        public String defaultNewline;        

        public Settings()
        {
            tempDirectory = Path.GetTempPath();
            bufferLines = 10000;
            defaultEncoding = Encoding.ASCII;
            defaultNewline = Environment.NewLine;
        }
    }
}