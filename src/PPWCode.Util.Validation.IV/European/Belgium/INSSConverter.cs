using System.Text.Json;
using System.Text.Json.Serialization;

namespace PPWCode.Util.Validation.IV.European.Belgium;

public class INSSConverter : JsonConverter<INSS>
{
    /// <inheritdoc />
    public override INSS Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Expected a string.");
        }

        string? inss = reader.GetString();
        if (string.IsNullOrEmpty(inss))
        {
            inss = string.Empty;
        }

        return new INSS(inss);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, INSS inss, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, inss.CleanedVersion, options);
}
