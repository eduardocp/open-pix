using OpenPix.Core.Infra; // Add the using directive

namespace OpenPix.Core.Domain;

public record Merchant
{
    public string Name { get; }
    public string City { get; }

    public Merchant(string name, string city)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(city);

        // 1. Remove diacritics
        var cleanName = name.RemoveDiacritics();
        var cleanCity = city.RemoveDiacritics();

        // 2. Truncate to maximum length allowed by EMV
        // Name: max 25 chars | City: max 15 chars
        Name = cleanName.Length > 25 ? cleanName[..25] : cleanName;
        City = cleanCity.Length > 15 ? cleanCity[..15] : cleanCity;
    }
}