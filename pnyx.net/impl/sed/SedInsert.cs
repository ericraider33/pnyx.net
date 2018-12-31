using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl.sed
{
    // https://linux.die.net/man/1/sed
    // http://www.grymoire.com/Unix/Sed.html#uh-0    
    public class SedInsert : ILineBuffering
    {
        public String text;
        
        public List<String> bufferingLine(String line)
        {
            return new List<String>
            {                
                text,
                line
            };            
        }

        public List<String> endOfFile()
        {
            return null;
        }
    }
}