using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl.sed
{
    // https://linux.die.net/man/1/sed
    // http://www.grymoire.com/Unix/Sed.html#uh-0    
    public class SedLineNumber : ILineBuffering
    {
        private int lineNumber;
        
        public List<String> bufferingLine(String line)
        {
            lineNumber++;            
            return new List<String>
            {
                lineNumber.ToString(),
                line
            };            
        }

        public List<String> endOfFile()
        {
            return null;
        }
    }
}