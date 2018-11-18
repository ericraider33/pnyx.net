using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl
{
    public class BeforeAfterBuffering : ILineBuffering
    {
        public int before;
        public int after;
        public ILineFilter lineFilter;
        public IRowFilter rowFilter;

        private String[] lineBuffer;
        private String[][] rowBuffer;
        private bool[] include;
        private int lineNumber;
        private readonly String[] resultLineBuffer = new string[1]; 
        private readonly String[][] resultRowBuffer = new string[1][]; 

        public BeforeAfterBuffering(int before, int after, ILineFilter lineFilter, IRowFilter rowFilter)
        {
            this.before = before;
            this.after = after;
            this.lineFilter = lineFilter;
            this.rowFilter = rowFilter;
            
            lineBuffer = new string[before+after+2];
            rowBuffer = new string[before+after+2][];
            include = new bool[before+after+2];
        }

        public string[] bufferingLine(string line)
        {
            lineNumber++;
            bool keep = lineFilter.shouldKeepLine(line);
            updateLine(keep);

            int indexToSet = getIndex(lineNumber);
            lineBuffer[indexToSet] = line;

            int lineToReturn = lineNumber - 1 - before;
            if (lineToReturn < 0)
                return null;

            int indexToReturn = getIndex(lineToReturn);
            if (!include[indexToReturn])
                return null;

            include[indexToReturn] = false;                                // clears flag for record
            resultLineBuffer[0] = lineBuffer[indexToReturn];
            return resultLineBuffer;
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

        public string[] endOfFile()
        {
            List<String> final = new List<String>();
            for (int lineToCheck = Math.Max(1, lineNumber - before); lineToCheck <= lineNumber; lineToCheck++)
            {
                int index = getIndex(lineToCheck);
                if (include[index])
                    final.Add(lineBuffer[index]);
            }

            return final.ToArray();
        }
    }
}