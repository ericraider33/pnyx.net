using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using pnyx.net.util;
using Xunit;

namespace pnyx.net.test.util;

public class EnumUtilTest
{
    public enum GenderEnum
    {
        Unspecified, Female, Male
    }

    private enum ToDictionaryTestEnum
    {
        Foo, Bar, Baz
    }

    private enum LabelTestEnum
    {
        TreeHouse,
        [Description("Google")]
        ABC,
    }

    [Fact]
    public void stringToEnum()
    {            
        Assert.Equal(GenderEnum.Unspecified, EnumUtil.stringToEnum<GenderEnum>(null));
        Assert.Equal(GenderEnum.Unspecified, EnumUtil.stringToEnum<GenderEnum>(""));
            
        Assert.Throws<ArgumentException>(() => EnumUtil.stringToEnum<GenderEnum>("x"));
            
        Assert.Equal(GenderEnum.Male, EnumUtil.stringToEnum<GenderEnum>("male"));
        Assert.Equal(GenderEnum.Male, EnumUtil.stringToEnum<GenderEnum>("MALE"));
        Assert.Equal(GenderEnum.Male, EnumUtil.stringToEnum<GenderEnum>("Male"));
            
        Assert.Equal(GenderEnum.Female, EnumUtil.stringToEnum<GenderEnum>("female"));
        Assert.Equal(GenderEnum.Female, EnumUtil.stringToEnum<GenderEnum>("FEMALE"));
        Assert.Equal(GenderEnum.Female, EnumUtil.stringToEnum<GenderEnum>("Female"));            
    }

    [Fact]
    public void testToDictionary()
    {
        Dictionary<string, ToDictionaryTestEnum> expected = new Dictionary<string, ToDictionaryTestEnum>
        {
            {Enum.GetName(typeof(ToDictionaryTestEnum), ToDictionaryTestEnum.Foo), ToDictionaryTestEnum.Foo},
            {Enum.GetName(typeof(ToDictionaryTestEnum), ToDictionaryTestEnum.Bar), ToDictionaryTestEnum.Bar},
            {Enum.GetName(typeof(ToDictionaryTestEnum), ToDictionaryTestEnum.Baz), ToDictionaryTestEnum.Baz}
        };

        Dictionary<string, ToDictionaryTestEnum> actual = EnumUtil.toDictionary<ToDictionaryTestEnum>();

        String x = String.Join(",", expected.OrderBy(kv => kv.Key).Select(kv => kv.Key + ":" + kv.Value));
        String y = String.Join(",", actual.OrderBy(kv => kv.Key).Select(kv => kv.Key + ":" + kv.Value));
        Assert.Equal(x, y);
    }

    [Fact]
    public void getLabel()
    {
        Assert.Equal("Tree House", LabelTestEnum.TreeHouse.getLabel());
        Assert.Equal("Google", LabelTestEnum.ABC.getLabel());
    }
}