using System.Text.RegularExpressions;

namespace pnyx.net.util;

public static class DiagnosisUtil
{
    // https://www.johndcook.com/blog/2019/05/05/regex_icd_codes/
    private static readonly Regex PARTIAL_REGEX = new Regex(@"(^[a-zA-Z][\d][\da-zA-Z]?)|(^[a-zA-Z]$)");
    private static readonly Regex COMPLETE_REGEX = new Regex(@"^[a-zA-Z][\d][\da-zA-Z][.][\da-zA-Z]{0,4}$");

    public static bool isLikeIcd10Id(string? value)
    {
        if (value == null || !PARTIAL_REGEX.IsMatch(value))
            return false;

        return value.Length <= 3 || COMPLETE_REGEX.IsMatch(value);
    }

    public static string removeFormatFromId(string id)
    {
        if (id.Length <= 3 || id[3] != '.')
            return id;
        
        return id.Substring(0, 3) + id.Substring(4);
    }
    
    public static string formatDiagnosisId(string diagnosisId)
    {
        if (diagnosisId.Length <= 3)
            return diagnosisId;
        
        return $"{diagnosisId.Substring(0, 3)}.{diagnosisId.Substring(3)}";
    }
    
    public static string? formatDiagnosisIdNullable(string? diagnosisId)
    {
        if (diagnosisId == null)
            return null;

        return formatDiagnosisId(diagnosisId);
    }
}