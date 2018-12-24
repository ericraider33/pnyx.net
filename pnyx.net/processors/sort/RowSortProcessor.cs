using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using pnyx.net.impl.csv;
using pnyx.net.util;

namespace pnyx.net.processors.sort
{
    public class RowSortProcessor : IRowProcessor, IRowPart, IDisposable
    {
        public IRowProcessor next { get; private set; }       
        public int bufferSize { get; private set; }
        public String tempDirectory { get; private set; }
        public IComparer<String[]> comparer;
        private readonly SortedList<String[], String[]> buffer;
        private readonly String tempFileKey;
        private int fileNumber;
        private readonly List<String> sortFiles = new List<String>();
        private readonly List<String> tempFiles = new List<String>();

        public RowSortProcessor(String tempDirectory = null,
            IComparer<String[]> comparer = null, 
            int bufferSize = 10000
            )
        {
            this.bufferSize = bufferSize;
            this.tempDirectory = tempDirectory ?? Directory.GetCurrentDirectory();
            this.comparer = comparer;
            
            buffer = new SortedList<String[], String[]>(bufferSize, comparer);            
            tempFileKey = TextUtil.extractAlphaNumeric(Guid.NewGuid().ToString());
        }

        public void rowHeader(String[] header)
        {
            next.rowHeader(header);
        }

        public void processRow(String[] row)
        {
            buffer.Add(row, row);

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
                    using (CsvReader reader = new CsvReader(stream, Encoding.UTF8))
                    {
                        String[] row;
                        while ((row = reader.readRow()) != null)
                            next.processRow(row);
                    }
                }
            }                
            
            next.endOfFile();
        }

        public void setNext(IRowProcessor next)
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
            String fileName = String.Format("row_sort_{0}_{1}.txt", tempFileKey, ++fileNumber);
            String filePath = Path.Combine(tempDirectory, fileName);
            tempFiles.Add(filePath);            // master list of files to be cleaned up
            sortFiles.Add(filePath);            // transient files used to output sorted 

            using (FileStream stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
            {
                using (CsvWriter writer = new CsvWriter(stream, Encoding.UTF8))
                {
                    foreach (String[] row in buffer.Values)                        
                        writer.writeRow(row);
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
            String fileNameC = String.Format("row_sort_{0}_{1}.txt", tempFileKey, ++fileNumber);
            String filePathC = Path.Combine(tempDirectory, fileNameC);
            tempFiles.Add(filePathC);            // master list of files to be cleaned up

            FileStream streamA = null, streamB = null, streamC = null;
            CsvReader readerA = null, readerB = null;
            CsvWriter writer = null;

            try
            {
                streamA = new FileStream(filePathA, FileMode.Open, FileAccess.Read);
                streamB = new FileStream(filePathB, FileMode.Open, FileAccess.Read);
                streamC = new FileStream(filePathC, FileMode.CreateNew, FileAccess.Write);

                readerA = new CsvReader(streamA, Encoding.UTF8);
                readerB = new CsvReader(streamB, Encoding.UTF8);
                writer = new CsvWriter(streamC, Encoding.UTF8);

                String[] rowA = null;
                String[] rowB = null;
                while (!readerA.EndOfStream || !readerB.EndOfStream)
                {
                    if (!readerA.EndOfStream && rowA == null)
                        rowA = readerA.readRow();
                    if (!readerB.EndOfStream && rowB == null)
                        rowB = readerB.readRow();
                    
                    if (rowA != null && rowB != null)
                    {
                        if (comparer.Compare(rowA, rowB) <= 0)
                        {
                            writer.writeRow(rowA);
                            rowA = null;
                        }
                        else
                        {
                            writer.writeRow(rowB);
                            rowB = null;
                        }
                    }
                    else if (rowA != null)
                    {
                        writer.writeRow(rowA);
                        rowA = null;
                    }
                    else if (rowB != null)
                    {
                        writer.writeRow(rowB);
                        rowB = null;
                    }
                }
                
                // Clears out any remaining lines
                if (rowA != null)
                    writer.writeRow(rowA);
                if (rowB != null)
                    writer.writeRow(rowB);                
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