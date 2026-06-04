using System;
using pnyx.net.errors;
using pnyx.net.util.dates;
using Xunit;

namespace pnyx.net.test.util.dates;

public class DateRangeTest
{
    [Fact]
    public void ToString_TypeOnly()
    {
        DateRange range = new DateRange { type = LocalRangeEnum.Last7Days };
        Assert.Equal("30", range.ToString());
    }

    [Fact]
    public void ToString_WithDates()
    {
        DateRange range = new DateRange
        {
            type = LocalRangeEnum.CustomRange,
            startDate = new DateTime(2023, 1, 1),
            endDate = new DateTime(2023, 1, 31)
        };
        Assert.Equal("93|2023-01-01|2023-01-31", range.ToString());
    }

    [Fact]
    public void ToString_PartialDates_ReturnsTypeOnly()
    {
        DateRange range = new DateRange
        {
            type = LocalRangeEnum.CustomRange,
            startDate = new DateTime(2023, 1, 1)
        };
        Assert.Equal("93|2023-01-01", range.ToString());

        range.startDate = null;
        range.endDate = new DateTime(2023, 1, 31);
        Assert.Equal("93||2023-01-31", range.ToString());
    }

    [Fact]
    public void Parse_NullOrEmpty()
    {
        Assert.Null(DateRange.parse(null));
        Assert.Null(DateRange.parse(""));
    }

    [Fact]
    public void Parse_TypeOnly()
    {
        DateRange? range = DateRange.parse("30");
        Assert.NotNull(range);
        Assert.Equal(LocalRangeEnum.Last7Days, range!.type);
        Assert.Null(range.startDate);
        Assert.Null(range.endDate);
    }

    [Fact]
    public void Parse_WithStartDate()
    {
        DateRange? range = DateRange.parse("93|2023-01-01");
        Assert.NotNull(range);
        Assert.Equal(LocalRangeEnum.CustomRange, range!.type);
        Assert.Equal(new DateTime(2023, 1, 1), range.startDate);
        Assert.Null(range.endDate);
    }

    [Fact]
    public void Parse_WithEndDate()
    {
        DateRange? range = DateRange.parse("93||2023-01-01");
        Assert.NotNull(range);
        Assert.Equal(LocalRangeEnum.CustomRange, range!.type);
        Assert.Null(range.startDate);
        Assert.Equal(new DateTime(2023, 1, 1), range.endDate);
    }

    [Fact]
    public void Parse_WithStartEndDates()
    {
        DateRange? range = DateRange.parse("93|2023-01-01|2023-01-31");
        Assert.NotNull(range);
        Assert.Equal(LocalRangeEnum.CustomRange, range!.type);
        Assert.Equal(new DateTime(2023, 1, 1), range.startDate);
        Assert.Equal(new DateTime(2023, 1, 31), range.endDate);
    }

    [Fact]
    public void Parse_InvalidFormat_ThrowsParseException()
    {
        Assert.Throws<ParseException>(() => DateRange.parse("93|2023-01-01|2023-01-31|extra"));
    }

    [Fact]
    public void Parse_InvalidType_ThrowsParseException()
    {
        Assert.Throws<ParseException>(() => DateRange.parse("999"));
    }

    [Fact]
    public void Parse_InvalidDates_ReturnsNullDates()
    {
        Assert.Throws<ParseException>(() => DateRange.parse("93|invalid-date|2023-01-31"));
        Assert.Throws<ParseException>(() => DateRange.parse("93|2023-01-31|invalid-date"));
    }

    [Fact]
    public void ImplicitOperator_FromEnum()
    {
        DateRange range = LocalRangeEnum.Last7Days;
        Assert.Equal(LocalRangeEnum.Last7Days, range.type);
    }

    [Fact]
    public void ExplicitOperator_FromString()
    {
        DateRange range = (DateRange)"30";
        Assert.Equal(LocalRangeEnum.Last7Days, range.type);

        Assert.Throws<ParseException>(() => (DateRange)"");
    }
}