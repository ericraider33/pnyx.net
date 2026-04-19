using System;
using System.Text;
using System.Threading.Tasks;
using pnyx.net.util;

namespace pnyx.net.processors.dest;

public class CaptureText : ILineProcessor
{
    public StringBuilder capture { get; private set; }
    public StreamInformation streamInformation { get; private set; }

    public CaptureText(StreamInformation streamInformation, StringBuilder? capture = null)
    {
        this.streamInformation = streamInformation;
        this.capture = capture ?? new StringBuilder();
    }

    public Task processLine(String line)
    {
        capture.Append(line);
        capture.Append(streamInformation.getOutputNewline());
        return Task.CompletedTask;
    }

    public Task endOfFile()
    {
        // Removes last line ending in capture buffer if original stream did NOT have a trailing newline 
        if (!streamInformation.endsWithNewLine && capture.Length > 0)
            capture.Length = capture.Length - streamInformation.getOutputNewline().Length;

        return Task.CompletedTask;
    }
}