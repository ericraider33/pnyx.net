using System;
using pnyx.net.fluent;
using pnyx.net.impl.columns;
using Xunit;
using Xunit.Abstractions;

namespace pnyx.net.test.impl.columns;

public class HasColumnIndexesTest : BaseTest
{
    public HasColumnIndexesTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }
    
    [Fact]
    public void basic()
    {
        String actual;
        using (Pnyx p = new Pnyx())
        {
            p.readString(PLANETS_GODS);
            p.parseCsv();
            p.hasColumns(true, RowConstants.A);
            actual = p.processToString();
        }

        Assert.Equal(PLANETS_GODS, actual);
    }
    
    [Fact]
    public void column_missing()
    {
        string columnsMissing =
            @"Aphrodite,Venus,""Goddess of love and beauty""
Ares,Mars
Cronus,Saturn
Hermes,Mercury,""Messenger of the gods, escort of souls to Hades""
Poseidon,Neptune
Uranus,Uranus
Zeus,Jupiter,""Sky god, supreme ruler of the Olympians""
";
    
        string columnsMissingExpected =
            @"Aphrodite,Venus,""Goddess of love and beauty""
Hermes,Mercury,""Messenger of the gods, escort of souls to Hades""
Zeus,Jupiter,""Sky god, supreme ruler of the Olympians""
";
        
        String actual;
        using (Pnyx p = new Pnyx())
        {
            p.readString(columnsMissing);
            p.parseCsv();
            p.hasColumns(true, RowConstants.C);
            actual = p.processToString();
        }

        Assert.Equal(columnsMissingExpected, actual);
    }
    
    [Fact]
    public void column_without_data()
    {
        string columnsMissing =
            @"Aphrodite,Venus,""Goddess of love and beauty""
Ares,Mars,
Cronus,Saturn,
Hermes,Mercury,""Messenger of the gods, escort of souls to Hades""
Poseidon,Neptune,
Uranus,Uranus,
Zeus,Jupiter,""Sky god, supreme ruler of the Olympians""
";
    
        string columnsMissingExpected =
            @"Aphrodite,Venus,""Goddess of love and beauty""
Hermes,Mercury,""Messenger of the gods, escort of souls to Hades""
Zeus,Jupiter,""Sky god, supreme ruler of the Olympians""
";
        
        String actual;
        using (Pnyx p = new Pnyx())
        {
            p.readString(columnsMissing);
            p.parseCsv();
            p.hasColumns(true, RowConstants.C);
            actual = p.processToString();
        }

        Assert.Equal(columnsMissingExpected, actual);
    }
    
    [Fact]
    public void column_check_value()
    {
        string columnsMissing =
@"Aphrodite,Venus,""Goddess of love and beauty""
Ares,Mars,""Hated god of war""
Cronus,Saturn,""Titan sky god, supreme ruler of the titans""
Hermes,Mercury,""Messenger of the gods, escort of souls to Hades""
Poseidon,Neptune,""God of the sea and earthquakes""
Uranus,Uranus,""Father of the Titans""
Zeus,Jupiter,""Sky god, supreme ruler of the Olympians""
";
        
        string columnsMissingExpected =
@"Aphrodite,Venus,""Goddess of love and beauty""
Ares,Mars,""Hated god of war""
Cronus,Saturn,""Titan sky god, supreme ruler of the titans""
Hermes,Mercury,""Messenger of the gods, escort of souls to Hades""
Poseidon,Neptune,""God of the sea and earthquakes""
Zeus,Jupiter,""Sky god, supreme ruler of the Olympians""
";
        
        String actual;
        using (Pnyx p = new Pnyx())
        {
            p.readString(columnsMissing);
            p.parseCsv();
            p.hasColumns(x => x != null && x.Contains("god", StringComparison.OrdinalIgnoreCase), RowConstants.C);
            actual = p.processToString();
        }

        Assert.Equal(columnsMissingExpected, actual);
    }
}