using System;
using pnyx.net.api;

namespace pnyx.net.impl.sed
{
    // https://linux.die.net/man/1/sed
    // http://www.grymoire.com/Unix/Sed.html#uh-0    
    public class SedLineNumber : ILineBuffering
    {
        private int lineNumber;
        
        public string[] bufferingLine(string line)
        {
            lineNumber++;            
            return new string[]
            {
                lineNumber.ToString(),
                line
            };            
        }

        public String[] endOfFile()
        {
            return null;
        }
    }
}