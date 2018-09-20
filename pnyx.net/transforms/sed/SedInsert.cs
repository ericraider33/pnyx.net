using System;
using pnyx.net.api;

namespace pnyx.net.transforms.sed
{
    // https://linux.die.net/man/1/sed
    // http://www.grymoire.com/Unix/Sed.html#uh-0    
    public class SedInsert : ILineBuffering
    {
        public String text;
        
        public string[] bufferingLine(string line)
        {
            return new string[]
            {                
                text,
                line
            };            
        }

        public String[] endOfFile()
        {
            return null;
        }
    }
}