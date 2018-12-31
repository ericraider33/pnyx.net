using System;
using System.Collections.Generic;
using pnyx.net.api;

namespace pnyx.net.impl.sed
{
    // https://linux.die.net/man/1/sed
    // http://www.grymoire.com/Unix/Sed.html#uh-0    
    public class SedAppendRow : IRowBuffering
    {
        public List<String> text;

        public List<String> rowHeader(List<String> header)
        {
            return header;
        }

        public List<List<String>> bufferingRow(List<String> row)
        {
            return new List<List<String>>
            {                
                row,
                text
            };            
        }

        public List<List<String>> endOfFile()
        {
            return null;
        }
    }
}