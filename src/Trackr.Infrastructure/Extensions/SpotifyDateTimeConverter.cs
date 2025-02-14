using Newtonsoft.Json;
using System;
using System.Globalization;

public class SpotifyDateTimeConverter : JsonConverter<DateTime?>
{
    public override DateTime? ReadJson(JsonReader reader, Type objectType, DateTime? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        string? dateStr = reader.Value?.ToString();
        if (string.IsNullOrWhiteSpace(dateStr))
            return null;

        if (dateStr.Length == 4 && int.TryParse(dateStr, out int year))
        {
            return new DateTime(year, 1, 1);
        }

        if (DateTime.TryParse(dateStr, out DateTime date))
        {
            return date;
        }

        throw new JsonException($"Invalid date format: {dateStr}");
    }

    

    public override void WriteJson(JsonWriter writer, DateTime? value, JsonSerializer serializer)
    {
        if (value.HasValue)
        {
            writer.WriteValue(value.Value.ToString("yyyy-MM-dd")); 
        }
        else
        {
            writer.WriteNull();
        }
    }
}
