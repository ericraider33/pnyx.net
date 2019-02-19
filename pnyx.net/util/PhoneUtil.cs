using System;

namespace pnyx.net.util
{
    public static class PhoneUtil
    {
        public static String parsePhone(String phone, String defualtAreaCode = null)
        {
            phone = TextUtil.emptyAsNull(TextUtil.extractNumeric(phone));

            if (phone == null || phone.StartsWith("0"))
                return null;

            if (phone.StartsWith("1") && phone.Length >= 11)
                phone = phone.Substring(1);

            if (phone.Length == 7 && defualtAreaCode != null)
                phone = defualtAreaCode + phone;

            return phone.Length == 10 ? phone : null;
        }        
        
    }
}