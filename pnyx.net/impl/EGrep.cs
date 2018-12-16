using System;
using System.Text.RegularExpressions;
using pnyx.net.api;

namespace pnyx.net.impl
{
    public class EGrep : ILineFilter
    {
        public String expression { get; private set; }
        public bool caseSensitive { get; private set; }
        private Regex regex;

        public EGrep(string expression, bool caseSensitive)
        {
            this.expression = expression;
            this.caseSensitive = caseSensitive;

            RegexOptions options = RegexOptions.None;
            if (!caseSensitive)
                options |= RegexOptions.IgnoreCase;
            
            regex = new Regex(expression, options);
        }

        public bool shouldKeepLine(string line)
        {
            return regex.IsMatch(line);   
        }        
    }
}