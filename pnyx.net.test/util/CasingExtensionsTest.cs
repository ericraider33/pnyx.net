using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.util;

public class CasingExtensionsTest
{
    [Fact]
    public void snakeToCamel()
    {
        Assert.Equal("DbVersion", CasingExtensions.snakeToCamel("db_version"));
    }

    [Fact]
    public void camelToKebob()
    {
        Assert.Equal("db-version", CasingExtensions.camelToKebob("DbVersion"));
        Assert.Equal("chronic-care-iq", CasingExtensions.camelToKebob("ChronicCareIQ"));
        Assert.Equal("mib", CasingExtensions.camelToKebob("MIB"));
        Assert.Equal("iam-interface", CasingExtensions.camelToKebob("IAmInterface"));
        Assert.Equal("mib-200", CasingExtensions.camelToKebob("MIB200"));
    }

    [Fact]
    public void camelToSpace()
    {
        Assert.Equal("x", CasingExtensions.camelToSpace("x"));
        Assert.Equal("x Y", CasingExtensions.camelToSpace("xY"));
        Assert.Equal("XYZ", CasingExtensions.camelToSpace("XYZ"));
        Assert.Equal("x Y", CasingExtensions.camelToSpace("x Y"));
        Assert.Equal("XYZoo Land", CasingExtensions.camelToSpace("XYZooLand"));
        Assert.Equal("Stay or Go", CasingExtensions.camelToSpace("StayOrGo"));
        Assert.Equal("Stand and Fight", CasingExtensions.camelToSpace("StandAndFight"));
        Assert.Equal("Would You Stay and Go or Go and Stay", CasingExtensions.camelToSpace("WouldYouStayAndGoOrGoAndStay"));

        Assert.Equal("x", CasingExtensions.camelToSpace("x"));
        Assert.Equal("x 1", CasingExtensions.camelToSpace("x1"));
        Assert.Equal("x 1", CasingExtensions.camelToSpace("x 1"));
        Assert.Equal("XYZoo 100", CasingExtensions.camelToSpace("XYZoo100"));
        Assert.Equal("Stay 100 Go", CasingExtensions.camelToSpace("Stay100Go"));
    }

    [Fact]
    public void spaceToCamel()
    {
        Assert.Equal("x", CasingExtensions.spaceToCamel("x"));
        Assert.Equal("xY", CasingExtensions.spaceToCamel("x Y"));
        Assert.Equal("XYZ", CasingExtensions.spaceToCamel("X Y Z"));
    }
}