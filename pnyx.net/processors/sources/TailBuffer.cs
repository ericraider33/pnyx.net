using System;
using pnyx.net.api;

namespace pnyx.net.processors.sources
{
    public class TailLineBuffer : BaseTailBuffer<String>, ILineBuffering
    {
        public TailLineBuffer(int bufferSize) : base(bufferSize)
        {
        }

        public String[] bufferingLine(String line)
        {
            addLineToBuffer(line);
            return null;
        }

        public String[] endOfFile()
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
            return getBuffer();
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

        protected T[] getBuffer()
        {
            int resultSize = Math.Min(lineNumber, buffer.Length);
            T[] result = new T[resultSize];

            int ln = lineNumber;
            for (int i = result.Length - 1; i >= 0; i--)
                result[i] = buffer[getIndex(ln--)];

            return result;
        }
    }
    
}