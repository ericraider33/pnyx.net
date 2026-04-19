using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace pnyx.net.util.dates;

public class DateConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (var jsonDoc = JsonDocument.ParseValue(ref reader))
        {
            string stringValue = jsonDoc.RootElement.GetRawText().Trim('"').Trim('\'');
            DateTime value = DateUtil.parseIso8601Date(stringValue); 
            return value;
        }
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.toIso8601Date());
    }
}
