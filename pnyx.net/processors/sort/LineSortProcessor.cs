using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using pnyx.net.util;

namespace pnyx.net.processors.sort
{
    public class LineSortProcessor : ILineProcessor, ILinePart, IDisposable
    {
        public ILineProcessor next { get; private set; }       
        public int bufferSize { get; private set; }
        public String tempDirectory { get; private set; }
        public IComparer<String> comparer { get; private set; }
        private readonly SortedList<String, String> buffer;
        private readonly String tempFileKey;
        private int fileNumber;
        private readonly List<String> sortFiles = new List<String>();
        private readonly List<String> tempFiles = new List<String>();

        public LineSortProcessor(bool unique = false,
            String tempDirectory = null,
            IComparer<String> comparer = null, 
            int bufferSize = 10000
            )
        {
            this.bufferSize = bufferSize;
            this.tempDirectory = tempDirectory ?? Directory.GetCurrentDirectory();
            this.comparer = comparer;
            
            buffer = new SortedList<String, String>(bufferSize, comparer);            
            tempFileKey = TextUtil.extractAlphaNumeric(Guid.NewGuid().ToString());
        }

        public void processLine(String line)
        {
            buffer.Add(line, line);

            if (buffer.Count >= bufferSize)
                emptyBuffer();
        }

        public void endOfFile()
        {
            if (buffer.Count > 0)
                emptyBuffer();

            sort();

            if (sortFiles.Count > 0)
            {
                using (FileStream stream = new FileStream(sortFiles[0], FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        String line;
                        while ((line = reader.ReadLine()) != null)
                            next.processLine(line);
                    }
                }
            }                
            
            next.endOfFile();
        }

        public void setNextLineProcessor(ILineProcessor next)
        {
            this.next = next;
        }

        public void Dispose()
        {
            foreach (String filePath in tempFiles)
            {
                FileInfo toRemove = new FileInfo(filePath);
                if (toRemove.Exists)
                    toRemove.Delete();                
            }
            
            tempFiles.Clear();
        }

        private void emptyBuffer()
        {
            String fileName = String.Format("line_sort_{0}_{1}.txt", tempFileKey, ++fileNumber);
            String filePath = Path.Combine(tempDirectory, fileName);
            tempFiles.Add(filePath);            // master list of files to be cleaned up
            sortFiles.Add(filePath);            // transient files used to output sorted 

            using (FileStream stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    foreach (String line in buffer.Values)                        
                        writer.WriteLine(line);
                }
            }
            
            buffer.Clear();
        }

        private void sort()
        {
            while (sortFiles.Count > 1)
            {
                for (int i = 0; i < sortFiles.Count-1; i++)
                {
                    String filePathA = sortFiles[i];
                    String filePathB = sortFiles[i + 1];

                    // Combines files
                    String filePathC = sortPair(filePathA, filePathB);
                    
                    // Replaces pair with resultant file
                    sortFiles[i] = filePathC;
                    sortFiles.RemoveAt(i+1);
                }
            }
        }

        private String sortPair(String filePathA, String filePathB)
        {
            String fileNameC = String.Format("line_sort_{0}_{1}.txt", tempFileKey, ++fileNumber);
            String filePathC = Path.Combine(tempDirectory, fileNameC);
            tempFiles.Add(filePathC);            // master list of files to be cleaned up

            FileStream streamA = null, streamB = null, streamC = null;
            StreamReader readerA = null, readerB = null;
            StreamWriter writer = null;

            try
            {
                streamA = new FileStream(filePathA, FileMode.Open, FileAccess.Read);
                streamB = new FileStream(filePathB, FileMode.Open, FileAccess.Read);
                streamC = new FileStream(filePathC, FileMode.CreateNew, FileAccess.Write);

                readerA = new StreamReader(streamA, Encoding.UTF8);
                readerB = new StreamReader(streamB, Encoding.UTF8);
                writer = new StreamWriter(streamC, Encoding.UTF8);

                String lineA = null;
                String lineB = null;
                while (!readerA.EndOfStream || !readerB.EndOfStream)
                {
                    if (!readerA.EndOfStream && lineA == null)
                        lineA = readerA.ReadLine();
                    if (!readerB.EndOfStream && lineB == null)
                        lineB = readerB.ReadLine();
                    
                    if (lineA != null && lineB != null)
                    {
                        if (comparer.Compare(lineA, lineB) <= 0)
                        {
                            writer.WriteLine(lineA);
                            lineA = null;
                        }
                        else
                        {
                            writer.WriteLine(lineB);
                            lineB = null;
                        }
                    }
                    else if (lineA != null)
                    {
                        writer.WriteLine(lineA);
                        lineA = null;
                    }
                    else if (lineB != null)
                    {
                        writer.WriteLine(lineB);
                        lineB = null;
                    }
                }
                
                // Clears out any remaining lines
                if (lineA != null)
                    writer.WriteLine(lineA);
                if (lineB != null)
                    writer.WriteLine(lineB);                
            }
            finally
            {
                if (readerA != null) readerA.Dispose();
                if (readerB != null) readerB.Dispose();
                if (writer != null) writer.Dispose();
                
                if (streamA != null) streamA.Dispose();
                if (streamB != null) streamB.Dispose();
                if (streamC != null) streamC.Dispose();
            }

            return filePathC;
        }
    }
}