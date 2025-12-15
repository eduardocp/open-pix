using OpenPix.Core.Domain;

namespace OpenPix.AspNetCore;

public interface IPixClient
{
    /// <summary>
    /// Cria um payload PIX usando as configurações padrão da aplicação.
    /// </summary>
    PixPayload CreatePayload(decimal amount, string? txId = null);

    /// <summary>
    /// Cria um payload PIX sobrescrevendo a chave (ex: para marketplaces).
    /// </summary>
    PixPayload CreatePayload(string pixKey, decimal amount, string? txId = null);
}