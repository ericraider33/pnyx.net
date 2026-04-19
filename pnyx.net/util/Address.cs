using System.Text;

namespace pnyx.net.util;

public class Address
{
    public string? Street { get; set; }
    public string? Street2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zipcode { get; set; }

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
