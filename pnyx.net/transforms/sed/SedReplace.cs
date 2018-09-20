using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using pnyx.net.api;
using pnyx.net.errors;
using pnyx.net.util;

namespace pnyx.net.transforms.sed
{
    public class SedReplace : ILineTransformer
    {
        public String pattern { get; private set; }
        public String replacement { get; private set; }
        public bool ignoreCase { get; private set; }
        public bool global { get; private set; }                
        public int? replaceIndex { get; private set; }
        public List<IndexRange> replaceRanges { get; private set; }

        private Regex regex;
        private StringBuilder builder = new StringBuilder();
        
        public SedReplace(string pattern, string replacement, string flags)
        {
            this.pattern = pattern;
            this.replacement = replacement;

            compileFlags(flags);
            
            RegexOptions options = RegexOptions.None;
            if (ignoreCase)
                options |= RegexOptions.IgnoreCase;
            
            regex = new Regex(pattern, options);
        }

        private static readonly Regex FLAG_PATTERN = new Regex("^([ig]*)([1-9,-]*)$");
        private void compileFlags(String flags)
        {
            if (flags == null)
            {
                replaceIndex = 1;
                return;
            }

            Match match = FLAG_PATTERN.Match(flags);            
            if (match == null)
                throw new InvalidArgumentException("Invalid flags: {0}", flags);

            String textFlags = match.Groups[1].Value;
            String textRanges = match.Groups[2].Value;

            ignoreCase = textFlags.Contains("i");
            global = textFlags.Contains("g");

            List<IndexRange> ranges = IndexRange.parse(textRanges);
            if (ranges.Count == 1 && ranges[0].isSingleIndex())
            {
                replaceIndex = ranges[0].low;
            }
            else if (ranges.Count > 0)
            {
                if (global)
                    throw new InvalidArgumentException("Global flag cannot be used with a range of replacement indexes");
                    
                replaceRanges = ranges;
            }
        }

        public string transformLine(string line)
        {
            Match match = regex.Match(line);
            if (!match.Success)
                return line;                    // doesn't match anything
            
            builder.Append(line);
            int matchIndex = 1;
            while (match.Success)
            {
                bool shouldReplace = false;
                if (global && replaceIndex == null && replaceRanges == null)
                    shouldReplace = true;                                        // global replace
                else if (replaceIndex.HasValue)
                {
                    if (global)
                        shouldReplace = matchIndex >= replaceIndex;
                    else
                        shouldReplace = matchIndex == replaceIndex;
                }
                else if (replaceRanges != null)
                {
                    //TODO
                }
                
                if (shouldReplace)
                    // Performs replacement
                    builder.Replace(match.Groups[0].Value, replacement, match.Groups[0].Index, match.Groups[0].Length);
                
                int startAt = match.Groups[0].Index + match.Groups[0].Length;
                match = regex.Match(line, startAt);
                matchIndex++;
            }

            String result = builder.ToString();
            builder.Clear();
            return result;
        }
    }
}