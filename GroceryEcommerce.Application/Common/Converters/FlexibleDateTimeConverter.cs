using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GroceryEcommerce.Application.Common.Converters;

/// <summary>
/// Converts incoming JSON values into <see cref="DateTime"/> regardless of whether
/// they are provided as ISO strings or Unix timestamps (seconds / milliseconds).
/// </summary>
public sealed class FlexibleDateTimeConverter : JsonConverter<DateTime?>
{
    private static readonly string[] SupportedFormats =
    [
        "yyyy-MM-dd HH:mm:ss",
        "yyyy/MM/dd HH:mm:ss",
        "dd/MM/yyyy HH:mm:ss",
        "MM/dd/yyyy HH:mm:ss",
        "yyyy-MM-ddTHH:mm:ss",
        "yyyy-MM-ddTHH:mm:ssK",
        "yyyy-MM-ddTHH:mm:ss.fff",
        "yyyy-MM-ddTHH:mm:ss.fffK"
    ];

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Null => null,
            JsonTokenType.String => ParseString(reader.GetString()),
            JsonTokenType.Number => ParseNumber(ref reader),
            _ => throw new JsonException($"Unsupported JSON token {reader.TokenType} for DateTime conversion.")
        };
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.Value.ToUniversalTime());
    }

    private static DateTime? ParseString(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (long.TryParse(value, out var numeric))
        {
            return FromUnixTimestamp(numeric);
        }

        foreach (var format in SupportedFormats)
        {
            if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var parsed))
            {
                return parsed.ToUniversalTime();
            }
        }

        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var fallback))
        {
            return fallback.ToUniversalTime();
        }

        throw new JsonException($"Cannot parse '{value}' to DateTime.");
    }

    private static DateTime? ParseNumber(ref Utf8JsonReader reader)
    {
        if (reader.TryGetInt64(out var numeric))
        {
            return FromUnixTimestamp(numeric);
        }

        if (reader.TryGetDouble(out var doubleValue))
        {
            var asLong = (long)doubleValue;
            return FromUnixTimestamp(asLong);
        }

        throw new JsonException("Cannot convert numeric JSON token to DateTime.");
    }

    private static DateTime FromUnixTimestamp(long value)
    {
        // Detect whether the value is in milliseconds (13 digits) or seconds (10 digits)
        var unixTime = value > 9_999_999_999 ? DateTimeOffset.FromUnixTimeMilliseconds(value) : DateTimeOffset.FromUnixTimeSeconds(value);
        return unixTime.UtcDateTime;
    }
}

