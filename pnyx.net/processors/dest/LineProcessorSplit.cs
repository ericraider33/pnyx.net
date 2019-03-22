using System;
using System.IO;
using pnyx.net.util;

namespace pnyx.net.processors.dest
{
    // p.writeSplit("icd10.$0.txt", 99, "c:/dev/pnyx.net/pnyx.net.test/files/tab");
    public class LineProcessorSplit : ILineProcessor, IDisposable
    {
        public readonly String path;
        public readonly int limit;
        public readonly String fileNamePattern;
        public readonly StreamInformation streamInformation;
        public TextWriter writer { get; private set; }

        private String previousLine;
        private int lineNumber;
        private int fileNumber;        
        
        public LineProcessorSplit(StreamInformation streamInformation, String fileNamePattern, int limit, String path)
        {
            this.streamInformation = streamInformation;
            this.fileNamePattern = fileNamePattern;
            this.limit = limit;
            this.path = path;
        }

        public void processLine(String line)
        {
            if (previousLine != null)
            {
                lineNumber++;
                if (lineNumber > limit)
                {
                    nextFile();
                    lineNumber = 1;
                }
                
                writer.Write(previousLine);
                writer.Write(streamInformation.getOutputNewline());
            }
            else
            {
                nextFile();
            }

            previousLine = line;
        }

        private void nextFile()
        {
            if (writer != null)
            {
                writer.Flush();
                writer.Close();
                writer.Dispose();
            }
            
            fileNumber++;

            String fileName = fileNamePattern.Replace("$0", String.Format("{0:000}", fileNumber));

            String combined = fileName;
            if (path != null)
                combined = Path.Combine(path, fileName);
                
            FileStream fs = new FileStream(combined, FileMode.Create, FileAccess.Write);
            writer = new StreamWriter(fs, streamInformation.getOutputEncoding());            
        }

        public void endOfFile()
        {
            if (previousLine != null)
            {              
                lineNumber++;
                if (lineNumber > limit)
                {
                    nextFile();
                    lineNumber = 1;
                }
                
                writer.Write(previousLine);
                if (streamInformation.endsWithNewLine)
                    writer.Write(streamInformation.getOutputNewline());
            }

            previousLine = null;
            
            if (writer != null)
                writer.Flush();
        }

        public void Dispose()
        {
            if (writer != null)
            {
                writer.Flush();
                writer.Dispose();
            }

            writer = null;
        }
    }
}