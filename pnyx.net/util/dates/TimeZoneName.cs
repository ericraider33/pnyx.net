using System;

namespace pnyx.net.util.dates;

public class TimeZoneName
{
    public String windowsId { get; }
    public String ianaId { get; }
    public String displayName { get; }

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
    public static readonly TimeZoneName UTC = new TimeZoneName("UTC",   TimeZoneConstants.UTC_IANA_ID, "(UTC+00:00) Coordinated Universal Time");
    
    public static readonly TimeZoneName Samoa = new TimeZoneName("UTC-11", "Pacific/Pago_Pago", "(UTC-11:00) Coordinated Universal Time-11");
    public static readonly TimeZoneName Aleutian = new TimeZoneName("Aleutian Standard Time", "America/Adak", "(UTC-10:00) Aleutian Islands");
    public static readonly TimeZoneName Marquesas = new TimeZoneName("Marquesas Standard Time", "Pacific/Marquesas", "(UTC-09:30) Marquesas Islands");
    public static readonly TimeZoneName Yukon = new TimeZoneName("Yukon Standard Time", "America/Whitehorse", "(UTC-07:00) Yukon");
    public static readonly TimeZoneName BajaCalifornia = new TimeZoneName("Pacific Standard Time (Mexico)", "America/Tijuana", "(UTC-08:00) Baja California");
    public static readonly TimeZoneName Chihuahua = new TimeZoneName("Mountain Standard Time (Mexico)", "America/Chihuahua", "(UTC-07:00) Chihuahua, La Paz, Mazatlan");
    public static readonly TimeZoneName CentralAmerica = new TimeZoneName("Central America Standard Time", "America/Guatemala", "(UTC-06:00) Central America");
    public static readonly TimeZoneName Saskatchewan = new TimeZoneName("Canada Central Standard Time", "America/Regina", "(UTC-06:00) Saskatchewan");
    public static readonly TimeZoneName Guadalajara = new TimeZoneName("Central Standard Time (Mexico)", "America/Mexico_City", "(UTC-06:00) Guadalajara, Mexico City, Monterrey");
    public static readonly TimeZoneName Bogota = new TimeZoneName("SA Pacific Standard Time", "America/Bogota", "(UTC-05:00) Bogota, Lima, Quito, Rio Branco");
    public static readonly TimeZoneName Havana = new TimeZoneName("Cuba Standard Time", "America/Havana", "(UTC-05:00) Havana");
    public static readonly TimeZoneName Indiana = new TimeZoneName("US Eastern Standard Time", "America/Indianapolis", "(UTC-05:00) Indiana (East)");
    public static readonly TimeZoneName Haiti = new TimeZoneName("Haiti Standard Time", "America/Port-au-Prince", "(UTC-05:00) Haiti");
    public static readonly TimeZoneName Asuncion = new TimeZoneName("Paraguay Standard Time", "America/Asuncion", "(UTC-04:00) Asuncion");
    public static readonly TimeZoneName Caracas = new TimeZoneName("Venezuela Standard Time", "America/Caracas", "(UTC-04:00) Caracas");
    public static readonly TimeZoneName Georgetown = new TimeZoneName("SA Western Standard Time", "America/Guyana", "(UTC-04:00) Georgetown, La Paz, Manaus, San Juan");
    public static readonly TimeZoneName Cuiaba = new TimeZoneName("Central Brazilian Standard Time", "America/Cuiaba", "(UTC-04:00) Cuiaba");
    public static readonly TimeZoneName Santiago = new TimeZoneName("Pacific SA Standard Time", "America/Santiago", "(UTC-04:00) Santiago");
    public static readonly TimeZoneName Newfoundland = new TimeZoneName("Newfoundland Standard Time", "America/St_Johns", "(UTC-03:30) Newfoundland");
    public static readonly TimeZoneName Brasilia = new TimeZoneName("E. South America Standard Time", "America/Sao_Paulo", "(UTC-03:00) Brasilia");
    public static readonly TimeZoneName BuenosAires = new TimeZoneName("Argentina Standard Time", "America/Argentina/Buenos_Aires", "(UTC-03:00) City of Buenos Aires");
    public static readonly TimeZoneName Cayenne = new TimeZoneName("SA Eastern Standard Time", "America/Cayenne", "(UTC-03:00) Cayenne, Fortaleza");
    public static readonly TimeZoneName Greenland = new TimeZoneName("Greenland Standard Time", "America/Nuuk", "(UTC-02:00) Greenland");
    public static readonly TimeZoneName CapeVerde = new TimeZoneName("Cape Verde Standard Time", "Atlantic/Cape_Verde", "(UTC-01:00) Cabo Verde Is.");
    public static readonly TimeZoneName Azores = new TimeZoneName("Azores Standard Time", "Atlantic/Azores", "(UTC-01:00) Azores");
    public static readonly TimeZoneName Casablanca = new TimeZoneName("Morocco Standard Time", "Africa/Casablanca", "(UTC+01:00) Casablanca");
    public static readonly TimeZoneName Greenwich = new TimeZoneName("Greenwich Standard Time", "Atlantic/Reykjavik", "(UTC+00:00) Monrovia, Reykjavik");
    public static readonly TimeZoneName GMT = new TimeZoneName("GMT Standard Time", "Europe/London", "(UTC+00:00) Dublin, Edinburgh, Lisbon, London");
    public static readonly TimeZoneName SaoTome = new TimeZoneName("Sao Tome Standard Time", "Africa/Sao_Tome", "(UTC+00:00) Sao Tome");
    public static readonly TimeZoneName WCentralAfrica = new TimeZoneName("W. Central Africa Standard Time", "Africa/Lagos", "(UTC+01:00) West Central Africa");
    public static readonly TimeZoneName CentralEurope = new TimeZoneName("Central Europe Standard Time", "Europe/Budapest", "(UTC+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague");
    public static readonly TimeZoneName CentralEuropean = new TimeZoneName("Central European Standard Time", "Europe/Warsaw", "(UTC+01:00) Sarajevo, Skopje, Warsaw, Zagreb");
    public static readonly TimeZoneName Romance = new TimeZoneName("Romance Standard Time", "Europe/Paris", "(UTC+01:00) Brussels, Copenhagen, Madrid, Paris");
    public static readonly TimeZoneName WEurope = new TimeZoneName("W. Europe Standard Time", "Europe/Berlin", "(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna");
    public static readonly TimeZoneName Jordan = new TimeZoneName("Jordan Standard Time", "Asia/Amman", "(UTC+03:00) Amman");
    public static readonly TimeZoneName GTB = new TimeZoneName("GTB Standard Time", "Europe/Bucharest", "(UTC+02:00) Athens, Bucharest");
    public static readonly TimeZoneName MiddleEast = new TimeZoneName("Middle East Standard Time", "Asia/Beirut", "(UTC+02:00) Beirut");
    public static readonly TimeZoneName Egypt = new TimeZoneName("Egypt Standard Time", "Africa/Cairo", "(UTC+02:00) Cairo");
    public static readonly TimeZoneName EEurope = new TimeZoneName("E. Europe Standard Time", "Asia/Nicosia", "(UTC+02:00) E. Europe");
    public static readonly TimeZoneName FLE = new TimeZoneName("FLE Standard Time", "Europe/Kyiv", "(UTC+02:00) Helsinki, Kyiv, Riga, Sofia, Tallinn, Vilnius");
    public static readonly TimeZoneName Israel = new TimeZoneName("Israel Standard Time", "Asia/Jerusalem", "(UTC+02:00) Jerusalem");
    public static readonly TimeZoneName Kaliningrad = new TimeZoneName("Kaliningrad Standard Time", "Europe/Kaliningrad", "(UTC+02:00) Kaliningrad");
    public static readonly TimeZoneName Libya = new TimeZoneName("Libya Standard Time", "Africa/Tripoli", "(UTC+02:00) Tripoli");
    public static readonly TimeZoneName SouthAfrica = new TimeZoneName("South Africa Standard Time", "Africa/Johannesburg", "(UTC+02:00) Harare, Pretoria");
    public static readonly TimeZoneName Sudan = new TimeZoneName("Sudan Standard Time", "Africa/Khartoum", "(UTC+02:00) Khartoum");
    public static readonly TimeZoneName SouthSudan = new TimeZoneName("South Sudan Standard Time", "Africa/Juba", "(UTC+02:00) Juba");
    public static readonly TimeZoneName Belarus = new TimeZoneName("Belarus Standard Time", "Europe/Minsk", "(UTC+03:00) Minsk");
    public static readonly TimeZoneName Arabic = new TimeZoneName("Arabic Standard Time", "Asia/Baghdad", "(UTC+03:00) Baghdad");
    public static readonly TimeZoneName Arab = new TimeZoneName("Arab Standard Time", "Asia/Riyadh", "(UTC+03:00) Kuwait, Riyadh");
    public static readonly TimeZoneName EAfrica = new TimeZoneName("E. Africa Standard Time", "Africa/Nairobi", "(UTC+03:00) Nairobi");
    public static readonly TimeZoneName Iran = new TimeZoneName("Iran Standard Time", "Asia/Tehran", "(UTC+03:30) Tehran");
    public static readonly TimeZoneName Arabian = new TimeZoneName("Arabian Standard Time", "Asia/Dubai", "(UTC+04:00) Abu Dhabi, Muscat");
    public static readonly TimeZoneName Astrakhan = new TimeZoneName("Astrakhan Standard Time", "Europe/Astrakhan", "(UTC+04:00) Astrakhan, Ulyanovsk");
    public static readonly TimeZoneName Azerbaijan = new TimeZoneName("Azerbaijan Standard Time", "Asia/Baku", "(UTC+04:00) Baku");
    public static readonly TimeZoneName RussiaTimeZone3 = new TimeZoneName("Russia Time Zone 3", "Europe/Samara", "(UTC+04:00) Izhevsk, Samara");
    public static readonly TimeZoneName Mauritius = new TimeZoneName("Mauritius Standard Time", "Indian/Mauritius", "(UTC+04:00) Port Louis");
    public static readonly TimeZoneName Saratov = new TimeZoneName("Saratov Standard Time", "Europe/Saratov", "(UTC+04:00) Saratov");
    public static readonly TimeZoneName Caucasus = new TimeZoneName("Caucasus Standard Time", "Asia/Yerevan", "(UTC+04:00) Yerevan");
    public static readonly TimeZoneName Afghanistan = new TimeZoneName("Afghanistan Standard Time", "Asia/Kabul", "(UTC+04:30) Kabul");
    public static readonly TimeZoneName WestAsia = new TimeZoneName("West Asia Standard Time", "Asia/Tashkent", "(UTC+05:00) Ashgabat, Tashkent");
    public static readonly TimeZoneName Ekaterinburg = new TimeZoneName("Ekaterinburg Standard Time", "Asia/Yekaterinburg", "(UTC+05:00) Ekaterinburg");
    public static readonly TimeZoneName Pakistan = new TimeZoneName("Pakistan Standard Time", "Asia/Karachi", "(UTC+05:00) Islamabad, Karachi");
    public static readonly TimeZoneName India = new TimeZoneName("India Standard Time", "Asia/Kolkata", "(UTC+05:30) Chennai, Kolkata, Mumbai, New Delhi");
    public static readonly TimeZoneName SriLanka = new TimeZoneName("Sri Lanka Standard Time", "Asia/Colombo", "(UTC+05:30) Sri Jayawardenepura");
    public static readonly TimeZoneName Nepal = new TimeZoneName("Nepal Standard Time", "Asia/Kathmandu", "(UTC+05:45) Kathmandu");
    public static readonly TimeZoneName CentralAsia = new TimeZoneName("Central Asia Standard Time", "Asia/Almaty", "(UTC+06:00) Astana");
    public static readonly TimeZoneName Bangladesh = new TimeZoneName("Bangladesh Standard Time", "Asia/Dhaka", "(UTC+06:00) Dhaka");
    public static readonly TimeZoneName Omsk = new TimeZoneName("Omsk Standard Time", "Asia/Omsk", "(UTC+06:00) Omsk");
    public static readonly TimeZoneName Myanmar = new TimeZoneName("Myanmar Standard Time", "Asia/Yangon", "(UTC+06:30) Yangon (Rangoon)");
    public static readonly TimeZoneName SEAsia = new TimeZoneName("SE Asia Standard Time", "Asia/Bangkok", "(UTC+07:00) Bangkok, Hanoi, Jakarta");
    public static readonly TimeZoneName Altai = new TimeZoneName("Altai Standard Time", "Asia/Barnaul", "(UTC+07:00) Barnaul, Gorno-Altaysk");
    public static readonly TimeZoneName NorthAsia = new TimeZoneName("North Asia Standard Time", "Asia/Krasnoyarsk", "(UTC+07:00) Krasnoyarsk");
    public static readonly TimeZoneName NCentralAsia = new TimeZoneName("N. Central Asia Standard Time", "Asia/Novosibirsk", "(UTC+07:00) Novosibirsk");
    public static readonly TimeZoneName China = new TimeZoneName("China Standard Time", "Asia/Shanghai", "(UTC+08:00) Beijing, Chongqing, Hong Kong SAR, Urumqi");
    public static readonly TimeZoneName Singapore = new TimeZoneName("Singapore Standard Time", "Asia/Singapore", "(UTC+08:00) Kuala Lumpur, Singapore");
    public static readonly TimeZoneName WAustralia = new TimeZoneName("W. Australia Standard Time", "Australia/Perth", "(UTC+08:00) Perth");
    public static readonly TimeZoneName Taipei = new TimeZoneName("Taipei Standard Time", "Asia/Taipei", "(UTC+08:00) Taipei");
    public static readonly TimeZoneName Ulaanbaatar = new TimeZoneName("Ulaanbaatar Standard Time", "Asia/Ulaanbaatar", "(UTC+08:00) Ulaanbaatar");
    public static readonly TimeZoneName NorthAsiaEast = new TimeZoneName("North Asia East Standard Time", "Asia/Irkutsk", "(UTC+08:00) Irkutsk");
    public static readonly TimeZoneName Transbaikal = new TimeZoneName("Transbaikal Standard Time", "Asia/Chita", "(UTC+09:00) Chita");
    public static readonly TimeZoneName Tokyo = new TimeZoneName("Tokyo Standard Time", "Asia/Tokyo", "(UTC+09:00) Osaka, Sapporo, Tokyo");
    public static readonly TimeZoneName Korea = new TimeZoneName("Korea Standard Time", "Asia/Seoul", "(UTC+09:00) Seoul");
    public static readonly TimeZoneName Yakutsk = new TimeZoneName("Yakutsk Standard Time", "Asia/Yakutsk", "(UTC+09:00) Yakutsk");
    public static readonly TimeZoneName CenAustralia = new TimeZoneName("Cen. Australia Standard Time", "Australia/Adelaide", "(UTC+09:30) Adelaide");
    public static readonly TimeZoneName AUSCentral = new TimeZoneName("AUS Central Standard Time", "Australia/Darwin", "(UTC+09:30) Darwin");
    public static readonly TimeZoneName EAustralia = new TimeZoneName("E. Australia Standard Time", "Australia/Brisbane", "(UTC+10:00) Brisbane");
    public static readonly TimeZoneName AUSEastern = new TimeZoneName("AUS Eastern Standard Time", "Australia/Sydney", "(UTC+10:00) Canberra, Melbourne, Sydney");
    public static readonly TimeZoneName WestPacific = new TimeZoneName("West Pacific Standard Time", "Pacific/Port_Moresby", "(UTC+10:00) Guam, Port Moresby");
    public static readonly TimeZoneName Tasmania = new TimeZoneName("Tasmania Standard Time", "Australia/Hobart", "(UTC+10:00) Hobart");
    public static readonly TimeZoneName Vladivostok = new TimeZoneName("Vladivostok Standard Time", "Asia/Vladivostok", "(UTC+10:00) Vladivostok");
    public static readonly TimeZoneName LordHowe = new TimeZoneName("Lord Howe Standard Time", "Australia/Lord_Howe", "(UTC+10:30) Lord Howe Island");
    public static readonly TimeZoneName Magadan = new TimeZoneName("Magadan Standard Time", "Asia/Magadan", "(UTC+11:00) Magadan");
    public static readonly TimeZoneName Sakhalin = new TimeZoneName("Sakhalin Standard Time", "Asia/Sakhalin", "(UTC+11:00) Sakhalin");
    public static readonly TimeZoneName CentralPacific = new TimeZoneName("Central Pacific Standard Time", "Pacific/Guadalcanal", "(UTC+11:00) Solomon Is., New Caledonia");
    public static readonly TimeZoneName RussiaTimeZone11 = new TimeZoneName("Russia Time Zone 11", "Asia/Kamchatka", "(UTC+12:00) Anadyr, Petropavlovsk-Kamchatsky");
    public static readonly TimeZoneName Fiji = new TimeZoneName("Fiji Standard Time", "Pacific/Fiji", "(UTC+12:00) Fiji");
    public static readonly TimeZoneName NewZealand = new TimeZoneName("New Zealand Standard Time", "Pacific/Auckland", "(UTC+12:00) Auckland, Wellington");
    public static readonly TimeZoneName Chatham = new TimeZoneName("Chatham Islands Standard Time", "Pacific/Chatham", "(UTC+12:45) Chatham Islands");
    public static readonly TimeZoneName Tonga = new TimeZoneName("Tonga Standard Time", "Pacific/Tongatapu", "(UTC+13:00) Nuku'alofa");
    public static readonly TimeZoneName LineIslands = new TimeZoneName("Line Islands Standard Time", "Pacific/Kiritimati", "(UTC+14:00) Kiritimati Island");
}