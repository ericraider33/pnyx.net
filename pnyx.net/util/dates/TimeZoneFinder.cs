using System;
using System.Collections.Generic;
using System.Linq;

namespace pnyx.net.util.dates;

public static class TimeZoneFinder
{
    private static readonly List<TimeZoneName> tzList = new()
    {
        TimeZoneName.Hawaii,
        TimeZoneName.Alaska,
        TimeZoneName.Pacific,
        TimeZoneName.Arizona,
        TimeZoneName.Mountain,
        TimeZoneName.Central,
        TimeZoneName.Eastern,
        TimeZoneName.GlaceBay,
        TimeZoneName.UTC,
        TimeZoneName.Samoa,
        TimeZoneName.Aleutian,
        TimeZoneName.Marquesas,
        TimeZoneName.Yukon,
        TimeZoneName.BajaCalifornia,
        TimeZoneName.Chihuahua,
        TimeZoneName.CentralAmerica,
        TimeZoneName.Saskatchewan,
        TimeZoneName.Guadalajara,
        TimeZoneName.Bogota,
        TimeZoneName.Havana,
        TimeZoneName.Indiana,
        TimeZoneName.Haiti,
        TimeZoneName.Asuncion,
        TimeZoneName.Caracas,
        TimeZoneName.Georgetown,
        TimeZoneName.Cuiaba,
        TimeZoneName.Santiago,
        TimeZoneName.Newfoundland,
        TimeZoneName.Brasilia,
        TimeZoneName.BuenosAires,
        TimeZoneName.Cayenne,
        TimeZoneName.Greenland,
        TimeZoneName.CapeVerde,
        TimeZoneName.Azores,
        TimeZoneName.Casablanca,
        TimeZoneName.Greenwich,
        TimeZoneName.GMT,
        TimeZoneName.SaoTome,
        TimeZoneName.WCentralAfrica,
        TimeZoneName.CentralEurope,
        TimeZoneName.CentralEuropean,
        TimeZoneName.Romance,
        TimeZoneName.WEurope,
        TimeZoneName.Jordan,
        TimeZoneName.GTB,
        TimeZoneName.MiddleEast,
        TimeZoneName.Egypt,
        TimeZoneName.EEurope,
        TimeZoneName.FLE,
        TimeZoneName.Israel,
        TimeZoneName.Kaliningrad,
        TimeZoneName.Libya,
        TimeZoneName.SouthAfrica,
        TimeZoneName.Sudan,
        TimeZoneName.SouthSudan,
        TimeZoneName.Belarus,
        TimeZoneName.Arabic,
        TimeZoneName.Arab,
        TimeZoneName.EAfrica,
        TimeZoneName.Iran,
        TimeZoneName.Arabian,
        TimeZoneName.Astrakhan,
        TimeZoneName.Azerbaijan,
        TimeZoneName.RussiaTimeZone3,
        TimeZoneName.Mauritius,
        TimeZoneName.Saratov,
        TimeZoneName.Caucasus,
        TimeZoneName.Afghanistan,
        TimeZoneName.WestAsia,
        TimeZoneName.Ekaterinburg,
        TimeZoneName.Pakistan,
        TimeZoneName.India,
        TimeZoneName.SriLanka,
        TimeZoneName.Nepal,
        TimeZoneName.CentralAsia,
        TimeZoneName.Bangladesh,
        TimeZoneName.Omsk,
        TimeZoneName.Myanmar,
        TimeZoneName.SEAsia,
        TimeZoneName.Altai,
        TimeZoneName.NorthAsia,
        TimeZoneName.NCentralAsia,
        TimeZoneName.China,
        TimeZoneName.Singapore,
        TimeZoneName.WAustralia,
        TimeZoneName.Taipei,
        TimeZoneName.Ulaanbaatar,
        TimeZoneName.NorthAsiaEast,
        TimeZoneName.Transbaikal,
        TimeZoneName.Tokyo,
        TimeZoneName.Korea,
        TimeZoneName.Yakutsk,
        TimeZoneName.CenAustralia,
        TimeZoneName.AUSCentral,
        TimeZoneName.EAustralia,
        TimeZoneName.AUSEastern,
        TimeZoneName.WestPacific,
        TimeZoneName.Tasmania,
        TimeZoneName.Vladivostok,
        TimeZoneName.LordHowe,
        TimeZoneName.Magadan,
        TimeZoneName.Sakhalin,
        TimeZoneName.CentralPacific,
        TimeZoneName.RussiaTimeZone11,
        TimeZoneName.Fiji,
        TimeZoneName.NewZealand,
        TimeZoneName.Chatham,
        TimeZoneName.Tonga,
        TimeZoneName.LineIslands,
    };
    private static readonly Dictionary<String, TimeZoneName> ianaidMap = tzList.ToDictionary(tzn => tzn.ianaId, tzn => tzn);
    private static readonly Dictionary<string, TimeZoneName> windowsMap = tzList.ToDictionary(tzn => tzn.windowsId, tzn => tzn);
    
    public static TimeZoneInfo getTimeZoneInfo(this TimeZoneName name)
    {
        return TimeZoneInfo.FindSystemTimeZoneById(Environment.OSVersion.Platform == PlatformID.Unix ? name.ianaId : name.windowsId);
    }
    
    /// <summary>
    /// Uses the IANA-ID to find TimeZoneInfo in a platform-independent way. If null is passed, then null is returned. For all other values, if
    /// the IANA-ID could not find a matching timezone, then an exception is thrown.
    /// </summary>
    /// <exception cref="InvalidTimeZoneException"></exception>
    public static TimeZoneInfo? findTimeZoneInfoByIanaIdNullable(string? ianaId)
    {
        if (ianaId == null)
            return null;
    
        TimeZoneName? tzn = ianaidMap.GetValueOrDefault(ianaId);
        if (tzn == null)
            throw new InvalidTimeZoneException($"Could not find a match for tz iana-ID={ianaId}");
        
        return TimeZoneInfo.FindSystemTimeZoneById(Environment.OSVersion.Platform == PlatformID.Unix ? tzn.ianaId : tzn.windowsId);
    }    
    
    /// <summary>
    /// Uses the IANA-ID to find TimeZoneInfo in a platform-independent way. If null is passed, then null is returned. For all other values, if
    /// the IANA-ID could not find a matching timezone, then an exception is thrown.
    /// </summary>
    /// <exception cref="InvalidTimeZoneException"></exception>
    public static TimeZoneInfo findTimeZoneInfoByIanaId(string ianaId)
    {
        TimeZoneName? tzn = ianaidMap.GetValueOrDefault(ianaId);
        if (tzn == null)
            throw new InvalidTimeZoneException($"Could not find a match for tz iana-ID={ianaId}");
        
        return TimeZoneInfo.FindSystemTimeZoneById(Environment.OSVersion.Platform == PlatformID.Unix ? tzn.ianaId : tzn.windowsId);
    }    
    
    /// <summary>
    /// Find the IANA-ID for a given TimeZone.
    /// </summary>
    /// <exception cref="InvalidTimeZoneException"></exception>
    public static string toIanaId(this TimeZoneInfo tz)
    {
        if (Environment.OSVersion.Platform == PlatformID.Unix)
            return tz.Id;

        TimeZoneName name = windowsMap[tz.Id];
        if (name == null)
            throw new InvalidTimeZoneException("Could not find a match for tz windowsId=" + tz.Id);

        return name.ianaId;
    } 
}
