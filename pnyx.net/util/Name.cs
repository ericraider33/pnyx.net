namespace pnyx.net.util;

public class Name
{
    public string? firstName { get; }
    public string? middleName { get; }
    public string? lastName { get; }
    public string? suffix { get; }

    public Name()
    {
    }
    
    public Name(string? firstName, string? lastName)
    {
        this.firstName = firstName;
        this.lastName = lastName;
    }
    
    public Name(string? firstName, string? middleName, string? lastName, string? suffix = null)
    {
        this.firstName = firstName;
        this.middleName = middleName;
        this.lastName = lastName;
        this.suffix = suffix;
    }

    public string toLastFirstTitle()
    {
        return NameUtil.toLastFirstTitle(this.firstName, this.middleName, this.lastName, this.suffix);
    }

    public string toFirstLastTitle()
    {
        return NameUtil.toFirstLastTitle(this.firstName, this.middleName, this.lastName, this.suffix);
    }

    public string toInitials()
    {
        return NameUtil.toInitials(this.firstName, this.middleName, this.lastName);
    }
    
}

public class SuffixName
{
    /// <summary>
    /// Name without the suffix, if any
    /// </summary>
    public string name { get; }
    
    /// <summary>
    /// Suffix parsed from name, if any
    /// </summary>
    public string? suffix { get; }

    public SuffixName(string name, string? suffix = null)
    {
        this.name = name;
        this.suffix = suffix;
    }
}