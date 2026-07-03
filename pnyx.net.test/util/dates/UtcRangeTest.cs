using System;
using pnyx.net.util.dates;
using Xunit;

namespace pnyx.net.test.util.dates;

public class UtcRangeTest
{
    [Fact]
    public void intersection_overlapping()
    {
        DateTime start1 = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime end1 = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc);
        DateTime start2 = new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc);
        DateTime end2 = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc);

        UtcRange range1 = new UtcRange(start1, end1);
        UtcRange range2 = new UtcRange(start2, end2);

        UtcRange? result = range1.intersection(range2);

        Assert.NotNull(result);
        Assert.Equal(start2, result!.startUtcDate.utc);
        Assert.Equal(end1, result.endUtcDate.utc);
    }

    [Fact]
    public void intersection_non_overlapping()
    {
        DateTime start1 = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime end1 = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc);
        DateTime start2 = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc);
        DateTime end2 = new DateTime(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc);

        UtcRange range1 = new UtcRange(start1, end1);
        UtcRange range2 = new UtcRange(start2, end2);

        UtcRange? result = range1.intersection(range2);
        Assert.Null(result);
    }

    [Fact]
    public void intersection_contained()
    {
        DateTime start1 = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime end1 = new DateTime(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc);
        DateTime start2 = new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc);
        DateTime end2 = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc);

        UtcRange range1 = new UtcRange(start1, end1);
        UtcRange range2 = new UtcRange(start2, end2);

        UtcRange? result = range1.intersection(range2);

        Assert.NotNull(result);
        Assert.Equal(start2, result!.startUtcDate.utc);
        Assert.Equal(end2, result!.endUtcDate.utc);
    }

    [Fact]
    public void union_overlapping()
    {
        DateTime start1 = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime end1 = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc);
        DateTime start2 = new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc);
        DateTime end2 = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc);

        UtcRange range1 = new UtcRange(start1, end1);
        UtcRange range2 = new UtcRange(start2, end2);

        UtcRange? result = range1.union(range2);

        Assert.NotNull(result);
        Assert.Equal(start1, result!.startUtcDate.utc);
        Assert.Equal(end2, result.endUtcDate.utc);
    }

    [Fact]
    public void union_non_overlapping()
    {
        DateTime start1 = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime end1 = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc);
        DateTime start2 = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc);
        DateTime end2 = new DateTime(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc);

        UtcRange range1 = new UtcRange(start1, end1);
        UtcRange range2 = new UtcRange(start2, end2);

        UtcRange? result = range1.union(range2);
        Assert.Null(result);
    }

    [Fact]
    public void testUnion_contained()
    {
        DateTime start1 = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime end1 = new DateTime(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc);
        DateTime start2 = new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc);
        DateTime end2 = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc);

        UtcRange range1 = new UtcRange(start1, end1);
        UtcRange range2 = new UtcRange(start2, end2);

        UtcRange? result = range1.union(range2);

        Assert.NotNull(result);
        Assert.Equal(start1, result!.startUtcDate.utc);
        Assert.Equal(end1, result.endUtcDate.utc);
    }
    
}