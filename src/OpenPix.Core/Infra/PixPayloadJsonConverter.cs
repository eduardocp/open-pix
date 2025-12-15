using System.Text.Json;
using System.Text.Json.Serialization;
using OpenPix.Core.Domain;

namespace OpenPix.Core.Infra;

public class PixPayloadJsonConverter : JsonConverter<PixPayload>
{
    public override PixPayload Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException("Deserialization of PixPayload is not directly supported. Use PixParser.Parse(string).");
    }

    public override void Write(Utf8JsonWriter writer, PixPayload value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        writer.WriteString("raw", value.RawString);
        
        if (value.Amount.HasValue)
            writer.WriteNumber("amount", value.Amount.Value);
        else
            writer.WriteNull("amount");

        if (!string.IsNullOrEmpty(value.Url))
        {
            writer.WriteString("type", "DYNAMIC");
            writer.WriteString("url", value.Url);
        }
        else
        {
            writer.WriteString("type", "STATIC");
            writer.WriteString("key", value.PixKey);
        }

        writer.WriteString("txId", value.TxId.Value);

        writer.WriteStartObject("merchant");
        writer.WriteString("name", value.Merchant?.Name);
        writer.WriteString("city", value.Merchant?.City);
        writer.WriteString("zipCode", value.Merchant?.ZipCode);
        writer.WriteEndObject();

        writer.WriteEndObject();
    }
}
