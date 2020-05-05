using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl
{
    public class SkipLastLineBuffering : BaseSkipLastBuffer<String>, ILineBuffering
    {
        public SkipLastLineBuffering(int bufferSize) : base(bufferSize)
        {
        }

        public List<String> bufferingLine(string line)
        {
            return addLineToBuffer(line);
        }

        public List<String> endOfFile()
        {
            return null;
        }
    }

    public class SkipLastRowBuffering : BaseSkipLastBuffer<List<String>>, IRowBuffering
    {
        public SkipLastRowBuffering(int bufferSize) : base(bufferSize)
        {
        }

        public List<String> rowHeader(List<String> header)
        {
            addLineToBuffer(header);
            return null;
        }

        public List<List<String>> bufferingRow(List<string> row)
        {
            return addLineToBuffer(row);
        }

        public List<List<String>> endOfFile()
        {
            return null;
        }
    }
    
    public abstract class BaseSkipLastBuffer<T> where T : class
    {
        private readonly T[] buffer;
        private int offset;

        protected BaseSkipLastBuffer(int bufferSize)
        {
            buffer = new T[bufferSize];
        }

        protected List<T> addLineToBuffer(T line)
        {
            T result = buffer[offset];            // fetches previous value - initially is NULL
            buffer[offset] = line;
            offset = (offset + 1) % buffer.Length;
            return new List<T> { result };
        }
    }
}