using System;
using pnyx.net.api;

namespace pnyx.net.impl.sed
{
    // https://linux.die.net/man/1/sed
    // http://www.grymoire.com/Unix/Sed.html#uh-0    
    public class SedAppendRow : IRowBuffering
    {
        public String[] text;

        public String[] rowHeader(String[] header)
        {
            return header;
        }

        public String[][] bufferingRow(String[] row)
        {
            return new String[][]
            {                
                row,
                text
            };            
        }

        public String[][] endOfFile()
        {
            return null;
        }
    }
}