using System;
using System.Collections.Generic;
using System.Linq;
using pnyx.net.util.dates;
using Xunit;

namespace pnyx.net.test.util.dates;

public class LocalDayRangeTest
{
    [Fact]
    public void getEnumerator_SingleDay()
    {
        TimeZoneInfo tz = TimeZoneInfo.Utc;
        LocalDay start = new LocalDay(tz, new DateTime(2024, 1, 1));
        LocalDay end = new LocalDay(tz, new DateTime(2024, 1, 2));
        LocalDayRange range = new LocalDayRange(start, end);

        string actual = string.Join(", ", range.Select(x => x.ToString()));
        Assert.Equal("2024-01-01", actual);
    }

    [Fact]
    public void getEnumerator_StartEqualsEnd()
    {
        TimeZoneInfo tz = TimeZoneInfo.Utc;
        LocalDay day = new LocalDay(tz, new DateTime(2024, 1, 1));
        LocalDayRange range = new LocalDayRange(day, day);

        string actual = string.Join(", ", range.Select(x => x.ToString()));
        Assert.Equal("", actual);
    }

    [Fact]
    public void getEnumerator_MultipleDays()
    {
        TimeZoneInfo tz = TimeZoneInfo.Utc;
        LocalDay start = new LocalDay(tz, new DateTime(2024, 1, 1));
        LocalDay end = new LocalDay(tz, new DateTime(2024, 1, 4));
        LocalDayRange range = new LocalDayRange(start, end);

        string actual = string.Join(", ", range.Select(x => x.ToString()));
        Assert.Equal("2024-01-01, 2024-01-02, 2024-01-03", actual);
    }

    [Fact]
    public void enumerable_ForEachSupport()
    {
        TimeZoneInfo tz = TimeZoneInfo.Utc;
        LocalDay start = new LocalDay(tz, new DateTime(2024, 1, 1));
        LocalDay end = new LocalDay(tz, new DateTime(2024, 1, 3));
        LocalDayRange range = new LocalDayRange(start, end);

        List<LocalDay> days = new List<LocalDay>();
        foreach (LocalDay day in range)
        {
            days.Add(day);
        }

        Assert.Equal(2, days.Count);
        Assert.Equal(start, days[0]);
        Assert.Equal(start.addDays(1), days[1]);
    }

    [Fact]
    public void getEnumerator_StartGreaterThanEnd()
    {
        TimeZoneInfo tz = TimeZoneInfo.Utc;
        LocalDay start = new LocalDay(tz, new DateTime(2024, 1, 2));
        LocalDay end = new LocalDay(tz, new DateTime(2024, 1, 1));
        LocalDayRange range = new LocalDayRange(start, end);

        string actual = string.Join(", ", range.Select(x => x.ToString()));
        Assert.Equal("", actual);
    }
}
