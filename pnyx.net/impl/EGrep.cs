using System;
using System.Text.RegularExpressions;
using pnyx.net.api;

namespace pnyx.net.impl;

public class EGrep : ILineFilter
{
    public String expression { get; }
    public bool caseSensitive { get;}
    
    private readonly Regex regex;

    public EGrep(String expression, bool caseSensitive)
    {
        this.expression = expression;
        this.caseSensitive = caseSensitive;

        RegexOptions options = RegexOptions.None;
        if (!caseSensitive)
            options |= RegexOptions.IgnoreCase;
            
        regex = new Regex(expression, options);
    }

    public bool shouldKeepLine(String line)
    {
        return regex.IsMatch(line);   
    }        
}