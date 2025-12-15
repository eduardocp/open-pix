namespace OpenPix.AspNetCore;

public class OpenPixOptions
{
    /// <summary>
    /// A Chave PIX padrão da empresa (Email, CPF, CNPJ, EVP).
    /// </summary>
    public string PixKey { get; set; } = string.Empty;

    /// <summary>
    /// Nome do beneficiário (ex: Nome da Loja). Máx 25 chars.
    /// </summary>
    public string MerchantName { get; set; } = string.Empty;

    /// <summary>
    /// Cidade do beneficiário. Máx 15 chars.
    /// </summary>
    public string City { get; set; } = string.Empty;
}