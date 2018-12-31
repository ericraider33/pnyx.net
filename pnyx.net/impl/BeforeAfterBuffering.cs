using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl
{
    public class BeforeAfterLineBuffering : BeforeAfterBase<String>, ILineBuffering
    {
        public ILineFilter lineFilter { get; }
        
        public BeforeAfterLineBuffering(int before, int after, ILineFilter lineFilter) : base(before, after)
        {
            this.lineFilter = lineFilter;
        }

        protected override bool shouldKeep(String source)
        {
            return lineFilter.shouldKeepLine(source);
        }

        public List<String> bufferingLine(String line)
        {
            return bufferingT(line);
        }

        public List<String> endOfFile()
        {
            return endOfFileT();
        }
    }
    
    public class BeforeAfterRowBuffering : BeforeAfterBase<String[]>, IRowBuffering
    {
        public IRowFilter rowFilter { get; }
        
        public BeforeAfterRowBuffering(int before, int after, IRowFilter rowFilter) : base(before, after)
        {
            this.rowFilter = rowFilter;
        }

        protected override bool shouldKeep(String[] source)
        {
            return rowFilter.shouldKeepRow(source);
        }

        public String[] rowHeader(String[] header)
        {
            return header;
        }

        public String[][] bufferingRow(String[] row)
        {
            List<String[]> result = bufferingT(row);
           return result == null ? null : result.ToArray();
        }

        public String[][] endOfFile()
        {
            return endOfFileT().ToArray();
        }
    }
    
    public abstract class BeforeAfterBase<T> where T : class
    {
        public int before { get; }
        public int after { get; }

        private T[] buffer;
        private bool[] include;
        private int lineNumber;
        private readonly List<T> resultBuffer = new List<T> { null }; 

        protected BeforeAfterBase(int before, int after)
        {
            this.before = before;
            this.after = after;
            
            buffer = new T[before+after+2];
            include = new bool[before+after+2];
        }

        protected abstract bool shouldKeep(T source);

        protected List<T> bufferingT(T line)
        {
            lineNumber++;
            bool keep = shouldKeep(line);
            updateLine(keep);

            int indexToSet = getIndex(lineNumber);
            buffer[indexToSet] = line;

            int lineToReturn = lineNumber - 1 - before;
            if (lineToReturn < 0)
                return null;

            int indexToReturn = getIndex(lineToReturn);
            if (!include[indexToReturn])
                return null;

            include[indexToReturn] = false;                                // clears flag for record
            resultBuffer[0] = buffer[indexToReturn];
            return resultBuffer;
        }

        private void updateLine(bool keep)
        {
            int index = getIndex(lineNumber);
            include[index] = include[index] || keep;                    // value may already be set from a previous entry with AFTER set

            if (!keep)
                return;
            
            for (int lineToSet = Math.Max(1, lineNumber - before); lineToSet < lineNumber; lineToSet++)
            {
                index = getIndex(lineToSet);
                include[index] = true;
            }
                            
            for (int lineToSet = lineNumber + after; lineToSet > lineNumber; lineToSet--)
            {
                index = getIndex(lineToSet);
                include[index] = true;
            }                
        }

        private int getIndex(int lineToFetch)
        {
            return lineToFetch % include.Length;
        }

        protected List<T> endOfFileT()
        {
            List<T> final = new List<T>();
            for (int lineToCheck = Math.Max(1, lineNumber - before); lineToCheck <= lineNumber; lineToCheck++)
            {
                int index = getIndex(lineToCheck);
                if (include[index])
                    final.Add(buffer[index]);
            }

            return final;
        }
    }
}