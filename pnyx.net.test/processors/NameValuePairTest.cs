using System;
using System.Collections.Generic;
using System.Linq;
using pnyx.net.errors;
using pnyx.net.fluent;
using Xunit;

namespace pnyx.net.test.processors;

public class NameValuePairTest
{
const String csvInputA = """
Title,Author,PublicationDate
Tale of Two Cities,Charles Dickens,1859
Oliver Twist,Charles Dickens,1839
Odyssey,Homer,-1000
""";
    
    [Fact]
    public void verify_basic_usage()
    {
        List<IDictionary<String, Object>> actual;
        using (Pnyx p = new Pnyx())
        {
            p.readString(csvInputA);
            p.parseCsv(hasHeader: true);
            p.rowToNameValuePair();
            actual = p.processCaptureNameValuePairs();
        }

        Assert.Equal(3, actual.Count);

        IDictionary<String, Object> first = actual[0];
        Assert.Equal("Author,PublicationDate,Title", String.Join(",", first.Keys.Order()));
        Assert.Equal("Tale of Two Cities", first["Title"]);
        Assert.Equal("Charles Dickens", first["Author"]);
        Assert.Equal("1859", first["PublicationDate"]);
    }

    [Fact]
    public void test_missing_header()
    {
        Pnyx p = new Pnyx();
        p.readString(csvInputA);
        p.parseCsv(hasHeader: false);
        p.rowToNameValuePair();
        Assert.Throws<IllegalStateException>(() => p.processCaptureNameValuePairs());
    }

    [Theory]
    [InlineData("simple", "simple")]
    [InlineData("with space", "withspace")]
    [InlineData("with-dash", "with-dash")]
    [InlineData("with_dash", "with_dash")]
    [InlineData("  trim  ", "trim")]
    public void header_clean_up(String header, String expected)
    {
        List<IDictionary<String, Object>> actual;
        using (Pnyx p = new Pnyx())
        {
            String source = $"{header}\nrecord";
            p.readString(source);
            p.parseCsv(hasHeader: true);
            p.rowToNameValuePair();
            actual = p.processCaptureNameValuePairs();
        }

        Assert.Equal(1, actual.Count);

        IDictionary<String, Object> first = actual[0];
        String propertyName = String.Join(",", first.Keys);
        Assert.Equal(expected, propertyName);
    }
    
    [Theory]
    [InlineData(-2000, 3)]
    [InlineData(1800, 2)]
    [InlineData(1845, 1)]
    [InlineData(1900, 0)]
    public void filter(int year, int expected)
    {
        List<IDictionary<String, Object>> actual;
        using (Pnyx p = new Pnyx())
        {
            p.readString(csvInputA);
            p.parseCsv(hasHeader: true);
            p.rowToNameValuePair();
            p.nameValuePairFilter(x => int.Parse((String)x["PublicationDate"]) >= year);
            actual = p.processCaptureNameValuePairs();
        }

        Assert.Equal(expected, actual.Count);
    }  
    
    [Fact]
    public void transform()
    {
        List<IDictionary<String, Object>> actual;
        using (Pnyx p = new Pnyx())
        {
            p.readString(csvInputA);
            p.parseCsv(hasHeader: true);
            p.rowToNameValuePair();
            p.nameValuePairTransformFunc(x =>
            {
                x.Remove("PublicationDate");
                return x;
            });
            actual = p.processCaptureNameValuePairs();
        }

        Assert.Equal(3, actual.Count);

        IDictionary<String, Object> first = actual[0];
        Assert.Equal("Author,Title", String.Join(",", first.Keys.Order()));
        Assert.Equal("Tale of Two Cities", first["Title"]);
        Assert.Equal("Charles Dickens", first["Author"]);
        Assert.False(first.ContainsKey("PublicationDate"));
    }
}