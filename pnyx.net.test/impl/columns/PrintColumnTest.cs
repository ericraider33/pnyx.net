using System.Threading.Tasks;
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
    public async Task basic()
    {
        string actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS);
            p.parseCsv();
            p.printColumn(2);
            actual = await p.processToString();
        }

        Assert.Equal(roman, actual);
    }
    
    [Fact]
    public async Task index()
    {
        string actual;
        await using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS);
            p.parseCsv();
            p.printColumn(RowConstants.B);
            actual = await p.processToString();
        }

        Assert.Equal(roman, actual);
    }
}