using System;

namespace pnyx.net.util;

/// <summary>
/// Class that represent an email address and an optional title
/// </summary>
public class EmailPair
{
    public string address { get; }
    public string? title { get; }
    
    public EmailPair(string address, string? title = null)
    {
        this.address = address;
        this.title = title.trimEmptyAsNull();
    }

    private bool Equals(EmailPair other)
    {
        return address == other.address && title == other.title;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((EmailPair)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(address, title);
    }

    public static implicit operator string(EmailPair emailPair)
    {
        return EmailUtil.formatRfc5322EmailAddress(emailPair.address, emailPair.title);
    }
    
    public static implicit operator EmailPair(string emailPair)
    {
        return EmailUtil.parseRfc5322EmailAddress(emailPair);
    }
    
    public override string ToString()
    {
        return EmailUtil.formatRfc5322EmailAddress(address, title);
    }

    public static EmailPair? fromNullable(string? emailAddress, string? title)
    {
        if (emailAddress == null)
            return null;
        
        return new EmailPair(emailAddress, title);
    }
}