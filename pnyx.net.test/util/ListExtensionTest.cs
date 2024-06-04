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
    }}