using System;

namespace pnyx.net.util;

public static class AddressUtil
{
    public static Address parse(String text)
    {
        if (String.IsNullOrWhiteSpace(text))
            return null;
            
        String[] parts = text.Split(new []{','}, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 3)
            return null;

        String zipText = parts[parts.Length - 1].Trim();
        if (!ZipCodeUtil.isZipCode(zipText))
            return null;
        zipText = ZipCodeUtil.parseZipCode(zipText);

        String stateText = parts[parts.Length - 2];
        stateText = UsaStateUtil.parseState(stateText);
        if (stateText == null)
            return null;

        if (parts.Length == 3)
            return new Address { Street = parts[0].Trim(), State = stateText, Zipcode = zipText };

        String cityText = parts[parts.Length - 3].Trim();
        if (parts.Length == 4)
            return new Address { Street = parts[0].Trim(), City = cityText, State = stateText, Zipcode = zipText };

        if (parts.Length == 5)
            return new Address { Street = parts[0].Trim(), Street2 = parts[1].Trim(), City = cityText, State = stateText, Zipcode = zipText };

        return null;
    }
}