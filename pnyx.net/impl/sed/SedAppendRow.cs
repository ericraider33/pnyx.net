using System;
using pnyx.net.api;

namespace pnyx.net.impl.sed
{
    // https://linux.die.net/man/1/sed
    // http://www.grymoire.com/Unix/Sed.html#uh-0    
    public class SedAppendRow : IRowBuffering
    {
        public String[] text;
        
        public string[][] bufferingRow(string[] row)
        {
            return new string[][]
            {                
                row,
                text
            };            
        }

        public string[][] endOfFile()
        {
            return null;
        }
    }
}