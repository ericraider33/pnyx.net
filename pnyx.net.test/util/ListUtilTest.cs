using System;
using System.Collections.Generic;
using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.util;

public class ListExtensionTest
{
    [Fact]
    public void addSorted()
    {
        Comparison<int> comparison = (a, b) => a - b;
        List<int> list = new();
        
        list.addSorted(comparison, 5);
        Assert.Equal("5", String.Join(",", list));
        
        list.addSorted(comparison, 1);
        Assert.Equal("1,5", String.Join(",", list));
        
        list.addSorted(comparison, 4);
        Assert.Equal("1,4,5", String.Join(",", list));
        
        list.addSorted(comparison, 7);
        Assert.Equal("1,4,5,7", String.Join(",", list));
    }
    
    [Fact]
    public void trunc()
    {
        List<int> list = new();

        // Empty list
        list.trunc(0);
        Assert.Empty(list);

        // Add elements
        list.AddRange(new[] { 1, 2, 3, 4, 5 });
        Assert.Equal("1,2,3,4,5", String.Join(",", list));

        // Truncate to smaller size
        list.trunc(3);
        Assert.Equal("1,2,3", String.Join(",", list));

        // Truncate to same size
        list.trunc(3);
        Assert.Equal("1,2,3", String.Join(",", list));

        // Truncate to larger size (should not change)
        list.trunc(10);
        Assert.Equal("1,2,3", String.Join(",", list));

        // Truncate to zero
        list.trunc(0);
        Assert.Empty(list);
    }
    
}