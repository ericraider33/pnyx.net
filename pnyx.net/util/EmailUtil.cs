using System;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace pnyx.net.util
{
    public class EmailUtil
    {
        private const string DOT_COM_PROVIDER_EXP = @".*[@].*((gmail)|(yahoo)|(hotmail)|(icloud)|(aol)|(msn)|(live)|(outlook)|(ymail)|(me))$";

        private static readonly String[] NO_EMAILS = new string[]
        {
            "NO@", "no_email", "nobody", "noeamil", "noemai", "noemial", "nomail", "nomemail"
        };

        public static String validateAndRepair(String emailAddr)
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

        public static bool isEmailAddress(String emailAddress)
        {
            if (String.IsNullOrEmpty(emailAddress))
                return false;

            try
            {
                MailAddress test = new MailAddress(emailAddress);
                
                Regex assureTld = new Regex(@".*[@][^.]+[.].+");  // TLD - top level domain (i.e. .com, .net, etc)
                return assureTld.Match(emailAddress).Success;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}