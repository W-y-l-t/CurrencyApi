using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.PublicApi.Models.Converters;

public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private const string DateFormat = "yyyy-MM-dd";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var date = reader.GetString();
        
        if (DateOnly.TryParseExact(
                date, 
                DateFormat, 
                CultureInfo.InvariantCulture, 
                DateTimeStyles.None, 
                out var dateOnly))
        {
            return dateOnly;
        }
        
        throw new JsonException($"Date format must be {DateFormat}.");
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
    }
}