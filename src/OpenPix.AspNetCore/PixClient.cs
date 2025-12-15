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
        // 1. Gera a string RAW usando as configurações injetadas
        var rawString = PixBuilder.Create()
            .WithKey(_options.PixKey)
            .WithMerchant(_options.MerchantName, _options.City)
            .WithAmount(amount)
            .WithTransactionId(txId ?? "***") // Se nulo, usa o padrão
            .Build();

        // 2. Converte a string num objeto rico usando o Parser
        return PixParser.Parse(rawString);
    }

    public PixPayload CreatePayload(string pixKey, decimal amount, string? txId = null)
    {
        // Versão com sobrecarga de chave (ex: Marketplace)
        var rawString = PixBuilder.Create()
            .WithKey(pixKey)
            .WithMerchant(_options.MerchantName, _options.City)
            .WithAmount(amount)
            .WithTransactionId(txId ?? "***")
            .Build();

        return PixParser.Parse(rawString);
    }
}