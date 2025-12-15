using System.Text.Json.Serialization;
using OpenPix.Core.Infra;

namespace OpenPix.Core.Domain;

[JsonConverter(typeof(PixPayloadJsonConverter))]
public record PixPayload(string RawString)
{
    public string? PixKey { get; init; }
    public string? Url { get; init; }
    public Merchant? Merchant { get; init; }
    public TransactionId TxId { get; init; } = TransactionId.Default;
    public decimal? Amount { get; init; }
    public string Description { get; init; } = string.Empty;


}