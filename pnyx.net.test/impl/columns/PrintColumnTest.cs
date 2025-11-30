using pnyx.net.fluent;
using pnyx.net.impl.columns;
using Xunit;
using Xunit.Abstractions;

namespace pnyx.net.test.impl.columns;

public class PrintColumnTest : BaseTest
{
    public PrintColumnTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }
    
    const string roman = 
@"Venus
Mars
Saturn
Mercury
Neptune
Uranus
Jupiter
";
    
    [Fact]
    public void basic()
    {
        
        string actual;
        using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS);
            p.parseCsv();
            p.printColumn(2);
            actual = p.processToString();
        }

        Assert.Equal(roman, actual);
    }
    
    [Fact]
    public void index()
    {
        
        string actual;
        using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS);
            p.parseCsv();
            p.printColumn(RowConstants.B);
            actual = p.processToString();
        }

        Assert.Equal(roman, actual);
    }
}