namespace pnyx.net.util;

public static class BoolUtil
{
    public const string Yes = "Yes";
    public const string No = "No";
    
    public static string toStringYesNo(this bool value)
    {
        return value ? Yes : No;
    }
    
    public static string? toStringYesNoNullable(this bool? value)
    {
        if (value == null)
            return null;

        return toStringYesNo(value.Value);
    }
}