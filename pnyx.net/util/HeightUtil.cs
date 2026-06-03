using System;

namespace pnyx.net.util;

public static class HeightUtil
{
    public static string? convertInchesToFeetNullable(int? inches)
    {
        if (inches == null)
            return null;
        
        return convertInchesToFeet(inches.Value);
    }

    public static string convertInchesToFeet(int inches)
    {
        int feetPart = inches / 12;
        int inchesPart = Math.Abs(inches % 12);
        return $"{feetPart}' {inchesPart}\"";
    }
    
}