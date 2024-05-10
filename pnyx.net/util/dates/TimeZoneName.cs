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

    public static readonly TimeZoneName Hawaii = new TimeZoneName("Hawaiian Standard Time",     "US/Hawaii",         "(UTC-10:00) Hawaii");
    public static readonly TimeZoneName Alaska = new TimeZoneName("Alaskan Standard Time",      "US/Alaska",         "(UTC-09:00) Alaska");
    public static readonly TimeZoneName Pacific = new TimeZoneName("Pacific Standard Time",     "US/Pacific",        "(UTC-08:00) Pacific Time (US & Canada)");
    public static readonly TimeZoneName Arizona = new TimeZoneName("US Mountain Standard Time", "US/Arizona",        "(UTC-07:00) Arizona");
    public static readonly TimeZoneName Mountain= new TimeZoneName("Mountain Standard Time",    "US/Mountain",       "(UTC-07:00) Mountain Time (US & Canada)");
    public static readonly TimeZoneName Central = new TimeZoneName("Central Standard Time",     "US/Central",        "(UTC-06:00) Central Time (US & Canada)");
    public static readonly TimeZoneName Eastern = new TimeZoneName("Eastern Standard Time",     "US/Eastern",        "(UTC-05:00) Eastern Time (US & Canada)");
    public static readonly TimeZoneName Indiana = new TimeZoneName("US Eastern Standard Time",  "US/East-Indiana",   "(UTC-05:00) Indiana (East)");
    public static readonly TimeZoneName GlaceBay = new TimeZoneName("Atlantic Standard Time",   "America/Glace_Bay", "(UTC-04:00) Atlantic Time (Canada)");
}