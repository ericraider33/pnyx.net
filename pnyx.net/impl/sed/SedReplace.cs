using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using pnyx.net.api;
using pnyx.net.errors;
using pnyx.net.util;

namespace pnyx.net.impl.sed
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
        private bool hasReplacementFormat = false;
        
        public SedReplace(string pattern, string replacement, string flags)
        {
            this.pattern = pattern;
            this.replacement = replacement;

            compileFlags(flags);
            
            RegexOptions options = RegexOptions.None;
            if (ignoreCase)
                options |= RegexOptions.IgnoreCase;            
            regex = new Regex(pattern, options);

            // Regex needs to be compiled first
            compileReplacementFormat();
        }

        private static readonly Regex FLAG_PATTERN = new Regex("^([ig]*)([0-9,-]*)$");
        private void compileFlags(String flags)
        {
            if (flags == null)
            {
                replaceIndex = 1;
                return;
            }

            Match match = FLAG_PATTERN.Match(flags);            
            if (!match.Success)
                throw new InvalidArgumentException("Invalid flags: {0}", flags);

            String textFlags = match.Groups[1].Value;
            String textRanges = match.Groups[2].Value;

            ignoreCase = textFlags.Contains("i");
            global = textFlags.Contains("g");

            List<IndexRange> ranges = IndexRange.parse(textRanges);
            if (ranges.Count == 1 && ranges[0].isSingleIndex())
            {
                replaceIndex = ranges[0].low;                
                if (replaceIndex <= 0)
                    throw new InvalidArgumentException("Invalid index: {0}. Must be greater than zero", replaceIndex);
            }
            else if (ranges.Count > 0)
            {
                if (global)
                    throw new InvalidArgumentException("Global flag cannot be used with a range of replacement indexes");
                    
                replaceRanges = ranges;

                foreach (IndexRange toCheck in replaceRanges)
                {
                    if (toCheck.high < toCheck.low)
                        throw new InvalidArgumentException("Invalid range: {0}. End index must be greater than start index", toCheck);
                        
                    if (toCheck.low <= 0)
                        throw new InvalidArgumentException("Invalid index: {0}. Must be greater than zero", toCheck.low);
                }
            }
        }

        private void compileReplacementFormat()
        {            
            int state = 0;
            int replacementCount = 0;
            foreach (char c in replacement)
            {
                if (state == 0)
                {
                    if (c == '\\')
                        state = 1;
                }
                else
                {
                    if (c >= '0' && c <= '9')
                    {                        
                        int groupNumber = c - '0';
                        replacementCount = Math.Max(replacementCount, groupNumber);
                    }
                    hasReplacementFormat = true;
                    state = 0;
                }
            }
            
            if (replacementCount+1 > regex.GetGroupNumbers().Length)                // adds 1 for '0' index
                throw new InvalidArgumentException("Invalid reference \\{0} on replace RHS", replacementCount);
        }

        public string transformLine(string line)
        {
            Match match = regex.Match(line);
            if (!match.Success)
                return line;                    // doesn't match anything
            
            builder.Append(line);
            int matchIndex = 1;
            int replacementOffset = 0;
            while (match.Success && match.Value.Length > 0)
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
                    shouldReplace = matchReplaceRanges(matchIndex);
                }

                if (shouldReplace)
                {
                    String actualText = replacement;
                    if (hasReplacementFormat)
                        actualText = generateReplacementText(match.Groups);
                                            
                    // Performs replacement
                    builder.Replace(match.Value, actualText, match.Index + replacementOffset, match.Length);
                    replacementOffset += actualText.Length - match.Length;            // adjusts for new length of text
                }
                
                int startAt = match.Index + match.Length;
                match = regex.Match(line, startAt);
                matchIndex++;
            }

            String result = builder.ToString();
            builder.Clear();
            return result;
        }

        private bool matchReplaceRanges(int matchIndex)
        {
            foreach (IndexRange toCheck in replaceRanges)
                if (toCheck.containsInclusive(matchIndex))
                    return true;
            
            return false;
        }
        
        private String generateReplacementText(GroupCollection groups)
        {            
            StringBuilder formatBuilder = new StringBuilder();
            int state = 0;
            foreach (char c in replacement)
            {
                if (state == 0)
                {
                    switch (c)
                    {
                        case '\\': state = 1; break;
                        default: formatBuilder.Append(c); break;
                    }
                }
                else
                {
                    switch (c)
                    {
                        case '\\': formatBuilder.Append('\\'); break;
                        case 'n': formatBuilder.Append('\n'); break;
                        case 'r': formatBuilder.Append('\r'); break;
                        case 't': formatBuilder.Append('\t'); break;
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                            int groupNumber = c - '0';
                            
                            if (groupNumber < groups.Count)
                                formatBuilder.Append(groups[groupNumber]);
                                                        
                            break;
                    }
                    state = 0;
                }
            }

            if (state == 1)
                formatBuilder.Append('\\');

            return formatBuilder.ToString();
        }
        
    }
}