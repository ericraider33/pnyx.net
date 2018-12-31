using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.processors.sources
{
    public class TailLineBuffer : BaseTailBuffer<String>, ILineBuffering
    {
        public TailLineBuffer(int bufferSize) : base(bufferSize)
        {
        }

        public List<String> bufferingLine(String line)
        {
            addLineToBuffer(line);
            return null;
        }

        public List<String> endOfFile()
        {
            return getBuffer();
        }
    }
    
    public class TailRowBuffer : BaseTailBuffer<String[]>, IRowBuffering
    {
        public TailRowBuffer(int bufferSize) : base(bufferSize)
        {
        }

        public String[] rowHeader(String[] header)
        {
            return header;
        }

        public String[][] bufferingRow(String[] row)
        {
            addLineToBuffer(row);
            return null;
        }

        public String[][] endOfFile()
        {
            return getBuffer().ToArray();
        }
    }

    public abstract class BaseTailBuffer<T> where T : class
    {
        private readonly T[] buffer;
        private int lineNumber;

        protected BaseTailBuffer(int bufferSize)
        {
            buffer = new T[bufferSize];
        }

        protected void addLineToBuffer(T line)
        {
            lineNumber++;
            buffer[getIndex(lineNumber)] = line;
        }

        private int getIndex(int ln)
        {
            return (ln - 1) % buffer.Length;
        }

        protected List<T> getBuffer()
        {
            int resultSize = Math.Min(lineNumber, buffer.Length);
            List<T> result = new List<T>(resultSize);

            for (int ln = lineNumber - resultSize + 1; ln <= lineNumber; ln++)
                result.Add(buffer[getIndex(ln)]);

            return result;
        }
    }
    
}