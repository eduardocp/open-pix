using Microsoft.Extensions.Options;
using OpenPix.Core;
using OpenPix.Core.Domain;

namespace OpenPix.AspNetCore;

public class PixClient : IPixClient
{
    private readonly OpenPixOptions _options;

    public PixClient(IOptions<OpenPixOptions> options)
    {
        _options = options.Value;

        if (string.IsNullOrWhiteSpace(_options.PixKey))
            throw new InvalidOperationException("OpenPix: 'PixKey' não foi configurada.");
    }

    public PixPayload CreatePayload(decimal amount, string? txId = null)
    {
        // 1. Generate the RAW string using injected configurations
        var rawString = PixBuilder.Create()
            .WithKey(_options.PixKey)
            .WithMerchant(_options.MerchantName, _options.City)
            .WithAmount(amount)
            .WithTransactionId(txId ?? "***") // If null, uses the default
            .Build();

        // 2. Converts the string into a rich object using the Parser
        return PixParser.Parse(rawString);
    }

    public PixPayload CreatePayload(string pixKey, decimal amount, string? txId = null)
    {
        // Version with key overload (e.g., Marketplace)
        var rawString = PixBuilder.Create()
            .WithKey(pixKey)
            .WithMerchant(_options.MerchantName, _options.City)
            .WithAmount(amount)
            .WithTransactionId(txId ?? "***")
            .Build();

        return PixParser.Parse(rawString);
    }
}