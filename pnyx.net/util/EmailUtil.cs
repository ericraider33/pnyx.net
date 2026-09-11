using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using pnyx.net.errors;

namespace pnyx.net.util;

public class EmailUtil
{
    private const string DOT_COM_PROVIDER_EXP = @".*[@].*((gmail)|(yahoo)|(hotmail)|(icloud)|(aol)|(msn)|(live)|(outlook)|(ymail)|(me))$";

    private static readonly String[] NO_EMAILS =
    [
        "NO@", "no_email", "nobody", "noeamil", "noemai", "noemial", "nomail", "nomemail"
    ];

    public static String? validateAndRepair(String? emailAddr)
    {
        String emailInRepair;

        // Not repairable
        if (string.IsNullOrWhiteSpace(emailAddr) || !emailAddr.Contains("@") && !emailAddr.Contains("."))
            return null;

        // Sub "," with "."
        Regex commaRegex = new Regex(@".*[@].*[,].*");
        if (commaRegex.Match(emailAddr).Success)
        {
            emailInRepair = emailAddr.Replace(",", ".");
            return isEmailAddress(emailInRepair) ? emailInRepair : null;
        }

        // Missing ".". Add it (if ending with "com" or "net")
        Regex missingComRegex = new Regex(@".*[@][^.]*((com)|(net))$", RegexOptions.IgnoreCase);
        if (missingComRegex.Match(emailAddr).Success)
        {
            emailInRepair = emailAddr.Insert(emailAddr.Length - 3, ".");
            return isEmailAddress(emailInRepair) ? emailInRepair : null;
        }

        // Check for known providers, whose addresses should end in ".com"
        Regex missingCom2Regex = new Regex(DOT_COM_PROVIDER_EXP, RegexOptions.IgnoreCase);
        if (missingCom2Regex.Match(emailAddr).Success)
        {
            emailInRepair = emailAddr + ".com";
            return isEmailAddress(emailInRepair) ? emailInRepair : null;
        }

        emailAddr = TextUtil.replaceEnding(emailAddr, ".c0m", ".com");
        emailAddr = TextUtil.replaceEnding(emailAddr, ".con", ".com");           

        if (!isEmailAddress(emailAddr))
            return null;

        // Checks commonly known NO email
        if (emailAddr.startsWithIgnoreCase("no") && TextUtil.startsWithAny(emailAddr, NO_EMAILS, ignoreCase: true))
            return null;

        return emailAddr;
    }

    public static bool isEmailAddress(String? emailAddress)
    {
        if (String.IsNullOrEmpty(emailAddress))
            return false;

        try
        {
            MailAddress test = new MailAddress(emailAddress);
            if (test == null)
                throw new NullReferenceException();
                
            Regex assureTld = new Regex(@".*[@][^.]+[.].+");  // TLD - top level domain (i.e. .com, .net, etc)
            return assureTld.Match(emailAddress).Success;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    /// <summary>
    /// Generates an RFC 5322 compliant email address, with optional title  
    /// </summary>
    public static string? formatRfc5322EmailAddressNullable(string? emailAddress, string? title = null)
    {
        if (string.IsNullOrEmpty(emailAddress))
            return null;

        return formatRfc5322EmailAddress(emailAddress, title);
    }   
    
    /// <summary>
    /// Generates an RFC 5322 compliant email address, with optional title  
    /// </summary>
    public static string formatRfc5322EmailAddress(string emailAddress, string? title = null)
    {
        if (string.IsNullOrEmpty(emailAddress))
            throw new InvalidArgumentException("Email address cannot be null or empty");

        if (string.IsNullOrWhiteSpace(title))
            return emailAddress;

        // Escape special characters and wrap in quotes if needed
        string displayName = title.Trim();

        // Check if title contains special characters that require quoting
        bool needsQuoting = displayName.Contains('"') || displayName.Contains('\\') || 
            displayName.Contains(',') || displayName.Contains(';') ||
            displayName.Contains('<') || displayName.Contains('>') ||
            displayName.Contains('@');

        if (needsQuoting)
        {
            // Escape backslashes and quotes
            displayName = displayName.Replace("\\", "\\\\").Replace("\"", "\\\"");
            displayName = $"\"{displayName}\"";
        }

        return $"{displayName} <{emailAddress}>";
    }

    /// <summary>
    /// Parse the title and email address from an RFC 5322 formatted email address  
    /// </summary>
    public static EmailPair? parseRfc5322EmailAddressNullable(string? emailAddress)
    {
        if (string.IsNullOrEmpty(emailAddress))
            return null;

        try
        {
            return parseRfc5322EmailAddress(emailAddress);
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    /// <summary>
    /// Parse the title and email address from an RFC 5322 formatted email address  
    /// </summary>
    public static EmailPair parseRfc5322EmailAddress(string emailAddress)
    {
        if (string.IsNullOrEmpty(emailAddress))
            throw new InvalidArgumentException("Email address cannot be null or empty");

        emailAddress = emailAddress.Trim();

        // Check if it's a simple email address without display name
        if (!emailAddress.Contains('<'))
        {
            if (!isEmailAddress(emailAddress))
                throw new InvalidArgumentException("Invalid email format: " + emailAddress);

            return new EmailPair(emailAddress);
        }

        // Extract display name and email parts
        int angleStart = emailAddress.IndexOf('<');
        int angleEnd = emailAddress.IndexOf('>');

        if (angleEnd == -1 || angleEnd < angleStart)
            throw new InvalidArgumentException("Invalid RFC 5322 email format");

        string displayNamePart = emailAddress.Substring(0, angleStart).Trim();
        string emailPart = emailAddress.Substring(angleStart + 1, angleEnd - angleStart - 1).Trim();
        if (!isEmailAddress(emailPart))
            throw new InvalidArgumentException("Invalid email format: " + emailPart);

        // Process display name - remove quotes and unescape
        string? displayName = null;
        if (!string.IsNullOrWhiteSpace(displayNamePart))
        {
            displayName = displayNamePart;

            // Remove surrounding quotes if present
            if (displayName.StartsWith("\"") && displayName.EndsWith("\"") && displayName.Length >= 2)
            {
                displayName = displayName.Substring(1, displayName.Length - 2);
                
                // Unescape escaped characters
                displayName = displayName.Replace("\\\"", "\"").Replace("\\\\", "\\");
            }
        }

        return new EmailPair(emailPart, displayName);
    }

    /// <summary>
    /// Parses a list of RFC 5322 email addresses.
    /// </summary>
    public static List<EmailPair> parseRfc5322EmailAddress(List<string> emailAddresses)
    {
        return emailAddresses.Select(parseRfc5322EmailAddress).ToList();
    }

    /// <summary>
    /// Parses a list of RFC 5322 email addresses.
    /// </summary>
    public static List<EmailPair>? parseRfc5322EmailAddressNullable(List<string>? emailAddresses)
    {
        if (emailAddresses == null)
            return null;
        
        return emailAddresses
            .Select(parseRfc5322EmailAddressNullable)
            .Where(x => x != null)
            .Cast<EmailPair>()
            .ToList();
    }
}
