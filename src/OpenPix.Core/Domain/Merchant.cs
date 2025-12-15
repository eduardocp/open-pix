using OpenPix.Core.Infra; // Adicione o using

namespace OpenPix.Core.Domain;

public record Merchant
{
    public string Name { get; }
    public string City { get; }

    public Merchant(string name, string city)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(city);

        // 1. Remove acentos
        var cleanName = name.RemoveDiacritics();
        var cleanCity = city.RemoveDiacritics();

        // 2. Trunca para o tamanho máximo permitido pelo EMV
        // Nome: máx 25 chars | Cidade: máx 15 chars
        Name = cleanName.Length > 25 ? cleanName[..25] : cleanName;
        City = cleanCity.Length > 15 ? cleanCity[..15] : cleanCity;
    }
}