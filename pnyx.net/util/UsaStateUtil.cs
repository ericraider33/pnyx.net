using System;
using System.Collections.Generic;

namespace pnyx.net.util;

public static class UsaStateUtil
{
    private static readonly HashSet<String> abbreviations = new(StringComparer.CurrentCultureIgnoreCase);
    private static readonly Dictionary<String, String> lookup = new(StringComparer.CurrentCultureIgnoreCase);

    static UsaStateUtil()
    {
        add("AL", "Alabama");
        add("AK", "Alaska");
        add("AZ", "Arizona");
        add("AR", "Arkansas");
        add("CA", "California");
        add("CZ", "CanalZone");
        add("CO", "Colorado");
        add("CT", "Connecticut");
        add("DE", "Delaware");
        add("DC", "DistrictofColumbia");
        add("FL", "Florida");
        add("GA", "Georgia");
        add("GU", "Guam");
        add("HI", "Hawaii");
        add("ID", "Idaho");
        add("IL", "Illinois");
        add("IN", "Indiana");
        add("IA", "Iowa");
        add("KS", "Kansas");
        add("KY", "Kentucky");
        add("LA", "Louisiana");
        add("ME", "Maine");
        add("MD", "Maryland");
        add("MA", "Massachusetts");
        add("MI", "Michigan");
        add("MN", "Minnesota");
        add("MS", "Mississippi");
        add("MO", "Missouri");
        add("MT", "Montana");
        add("NE", "Nebraska");
        add("NV", "Nevada");
        add("NH", "NewHampshire");
        add("NJ", "NewJersey");
        add("NM", "NewMexico");
        add("NY", "NewYork");
        add("NC", "NorthCarolina");
        add("ND", "NorthDakota");
        add("OH", "Ohio");
        add("OK", "Oklahoma");
        add("OR", "Oregon");
        add("PA", "Pennsylvania");
        add("PR", "PuertoRico");
        add("RI", "RhodeIsland");
        add("SC", "SouthCarolina");
        add("SD", "SouthDakota");
        add("TN", "Tennessee");
        add("TX", "Texas");
        add("UT", "Utah");
        add("VT", "Vermont");
        add("VI", "VirginIslands");
        add("VA", "Virginia");
        add("WA", "Washington");
        add("WV", "WestVirginia");
        add("WI", "Wisconsin");
        add("WY", "Wyoming");
    }

    private static void add(String abbreviation, String stateName)
    {
        abbreviations.Add(abbreviation);
        lookup.Add(stateName, abbreviation);
    }

    public static String parseState(String input)
    {
        if (String.IsNullOrEmpty(input))
            return null;

        if (input.endsWithIgnoreCase(" USA"))
            input = input.trunc(input.Length - 4);

        if (input.endsWithIgnoreCase(" US"))
            input = input.trunc(input.Length - 3);

        input = ParseExtensions.extractAlphaNumeric(input);

        if (abbreviations.Contains(input))
            return input.ToUpper();                // Fixes case

        if (lookup.ContainsKey(input))
            return lookup[input];

        return null;
    }
}