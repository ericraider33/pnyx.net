using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using pnyx.net.util.dates;
using Xunit;

namespace pnyx.net.test.util.dates;

public class DateConvertTest
{
    public class ObjectWithDate
    {
        public string? name { get; set; }
        [JsonConverter(typeof(DateConverter))]
        public DateTime dateOfBirth { get; set; }
    }

    [Fact]
    public void object_to_json()
    {
        ObjectWithDate x = new ObjectWithDate() { name = "EE", dateOfBirth = new DateTime(1975, 5, 29) };
        string asJson = JsonSerializer.Serialize(x);
        
        string expected = """{"name":"EE","dateOfBirth":"1975-05-29"}""";
        Assert.Equal(expected, asJson);
    }

    [Fact]
    public void json_to_object()
    {
        string json = """{"name":"EE","dateOfBirth":"1975-05-29"}""";
        ObjectWithDate? x = JsonSerializer.Deserialize<ObjectWithDate>(json);
        Assert.Equal("EE", x?.name);
        Assert.Equal(new DateTime(1975, 5, 29), x?.dateOfBirth);
    }
    
    public class ObjectWithDateOnly
    {
        public string? name { get; set; }
        public DateOnly dateOfBirth { get; set; }
    }

    [Fact]
    public void object_to_json_date_only()
    {
        ObjectWithDateOnly x = new ObjectWithDateOnly() { name = "EE", dateOfBirth = new DateOnly(1975, 5, 29) };
        string asJson = JsonSerializer.Serialize(x);
        
        string expected = """{"name":"EE","dateOfBirth":"1975-05-29"}""";
        Assert.Equal(expected, asJson);
    }

    [Fact]
    public void json_to_object_date_only()
    {
        string json = """{"name":"EE","dateOfBirth":"1975-05-29"}""";
        ObjectWithDateOnly? x = JsonSerializer.Deserialize<ObjectWithDateOnly>(json);
        Assert.Equal("EE", x?.name);
        Assert.Equal(new DateOnly(1975, 5, 29), x?.dateOfBirth);
    }
}