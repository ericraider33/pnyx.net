using System;
using System.Text;
using pnyx.net.util;

namespace pnyx.net.processors.dest
{
    public class CaptureText : ILineProcessor
    {
        public StringBuilder capture { get; private set; }
        public StreamInformation streamInformation { get; private set; }

        public CaptureText(StreamInformation streamInformation, StringBuilder capture = null)
        {
            this.streamInformation = streamInformation;
            this.capture = capture ?? new StringBuilder();
        }

        public void processLine(String line)
        {
            capture.Append(line);
            capture.Append(streamInformation.getNewline());
        }

        public void endOfFile()
        {
            if (!streamInformation.endsWithNewLine && capture.Length > 0)
                capture.Length = capture.Length - streamInformation.getNewline().Length;
        }
    }
}