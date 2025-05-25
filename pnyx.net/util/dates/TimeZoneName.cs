using System;

namespace pnyx.net.util.dates;

public class TimeZoneName
{
    public String windowsId { get; set; }
    public String ianaId { get; set; }
    public String displayName { get; set; }

    public TimeZoneName(string windowsId, string ianaId, string displayName)
    {
        this.windowsId = windowsId;
        this.ianaId = ianaId;
        this.displayName = displayName;
    }

    public static readonly TimeZoneName Hawaii = new TimeZoneName("Hawaiian Standard Time",     "Pacific/Honolulu",         "(UTC-10:00) Hawaii");
    public static readonly TimeZoneName Alaska = new TimeZoneName("Alaskan Standard Time",      "America/Anchorage",         "(UTC-09:00) Alaska");
    public static readonly TimeZoneName Pacific = new TimeZoneName("Pacific Standard Time",     "America/Los_Angeles",        "(UTC-08:00) Pacific Time (US & Canada)");
    public static readonly TimeZoneName Arizona = new TimeZoneName("US Mountain Standard Time", "America/Phoenix",        "(UTC-07:00) Arizona");
    public static readonly TimeZoneName Mountain= new TimeZoneName("Mountain Standard Time",    "America/Denver",       "(UTC-07:00) Mountain Time (US & Canada)");
    public static readonly TimeZoneName Central = new TimeZoneName("Central Standard Time",     "America/Chicago",        "(UTC-06:00) Central Time (US & Canada)");
    public static readonly TimeZoneName Eastern = new TimeZoneName("Eastern Standard Time",     "America/New_York",        "(UTC-05:00) Eastern Time (US & Canada)");
    public static readonly TimeZoneName GlaceBay = new TimeZoneName("Atlantic Standard Time",   "America/Glace_Bay", "(UTC-04:00) Atlantic Time (Canada)");
    public static readonly TimeZoneName UTC = new TimeZoneName("UTC",   "Etc/UTC", "(UTC+00:00) Coordinated Universal Time");
}