using System;
using pnyx.net.api;
using pnyx.net.util;

namespace pnyx.net.impl;

public class Grep : ILineFilter
{
    public String textToFind { get; }
    public bool caseSensitive { get; }

    public Grep(string textToFind, bool caseSensitive = false)
    {
        this.textToFind = textToFind;
        this.caseSensitive = caseSensitive;
    }

    public bool shouldKeepLine(String line)
    {
        bool match;
        if (caseSensitive)
            match = line.Contains(textToFind);
        else
            match = line.containsIgnoreCase(textToFind);

        return match;
    }
}