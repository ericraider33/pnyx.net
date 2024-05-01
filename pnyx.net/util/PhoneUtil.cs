using System;

namespace pnyx.net.util;

public static class PhoneUtil
{
    public static String parsePhone(String phone, String defualtAreaCode = null)
    {
        phone = ParseExtensions.extractNumeric(phone).emptyAsNull();

        if (phone == null || phone.StartsWith("0"))
            return null;

        if (phone.StartsWith("1") && phone.Length >= 11)
            phone = phone.Substring(1);

        if (phone.Length == 7 && defualtAreaCode != null)
            phone = defualtAreaCode + phone;

        return phone.Length == 10 ? phone : null;
    }        
    
    public static String formatPhone(String x)
    {
        x = x.trimEmptyAsNull();
        if (x == null)
            return null;

        if (x.Length < 7)
            return String.Concat("x", x);

        if (x.Length < 10)
            return x;

        String areaCode = x.Substring(0, 3);
        String switchCode = x.Substring(3, 3);
        String exchangeCode = x.Substring(6, 4).Trim();
        String result = $"({areaCode}) {switchCode}-{exchangeCode}";

        if (x.Length > 10)
            result = String.Join(" x", result, x.Substring(10));

        return result;
    }    
}