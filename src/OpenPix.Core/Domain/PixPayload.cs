namespace OpenPix.Core.Domain;

public record PixPayload
{
    public string RawString { get; }
    public string? PixKey { get; init; }
    public string? Url { get; init; }
    public Merchant? Merchant { get; init; }
    public TransactionId TxId { get; init; } = TransactionId.Default;
    public decimal? Amount { get; init; }
    public string Description { get; init; } = string.Empty;

    public PixPayload(string rawString)
    {
        RawString = rawString;
    }
}