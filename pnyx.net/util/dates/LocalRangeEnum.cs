namespace pnyx.net.util.dates;

public enum LocalRangeEnum
{
    Unspecified = 0,
    Today = 1,
    Yesterday = 2,

    Last7Days = 30,
    Last14Days = 31,
    Last30Days = 32,
    Last60Days = 33,
    Last90Days = 34,
    Last180Days = 35,
    Last365Days = 36,
    Last2Years = 40,
    Last3Years = 41,
    Last5Years = 42,
    Last10Years = 43,
    Last20Years = 44,
    Last30Years = 45,
    Last50Years = 46,
    Last100Years = 47,
    Last200Years = 48,
    
    ThisCalendarWeek = 60,
    LastCalendarWeek = 61,
    ThisCalendarMonth = 62,
    LastCalendarMonth = 63,
    ThisCalendarYear = 64,
    LastCalendarYear = 65,
    ThisCalendarDecade = 66,
    LastCalendarDecade = 67,
    ThisCalendarCentury = 68,
    LastCalendarCentury = 69,
    
    CustomMonth = 90,
    CustomDay = 91,
    CustomYear = 92,
    CustomRange = 93,
    
    Since1900 = 100,
    Since1970 = 101,
    Since2000 = 102,
    
    All1900 = 110,          // just like `Since`, but with a little nicer description 
    All1970 = 111,
    All2000 = 112,
}