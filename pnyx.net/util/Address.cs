using System;
using System.Text;

namespace pnyx.net.util;

public class Address
{
    public String Street { get; set; }
    public String Street2 { get; set; }
    public String City { get; set; }
    public String State { get; set; }
    public String Zipcode { get; set; }

    public override string ToString()
    {
        StringBuilder result = new StringBuilder();
        if (Street != null)
            result.Append(Street).Append("\n");
        if (Street2 != null)
            result.Append(Street2).Append("\n");
        if (City != null)
            result.Append(City).Append(", ");
        if (State != null)
            result.Append(State).Append(" ");
        if (Zipcode != null)
            result.Append(Zipcode);

        return result.ToString();
    }
}
