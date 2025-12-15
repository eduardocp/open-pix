using OpenPix.Core.Infra; // Add the using directive

namespace OpenPix.Core.Domain;

public record Merchant
{
    public string Name { get; }
    public string City { get; }
    public string? ZipCode { get; }

    public Merchant(string name, string city, string? zipCode = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(city);

        // 1. Remove diacritics
        var cleanName = name.RemoveDiacritics();
        var cleanCity = city.RemoveDiacritics();

        // 2. Truncate/Sanitize
        // Name: max 25 chars | City: max 15 chars | Zip: max 10 chars
        Name = cleanName.Length > 25 ? cleanName[..25] : cleanName;
        City = cleanCity.Length > 15 ? cleanCity[..15] : cleanCity;
        
        if (!string.IsNullOrWhiteSpace(zipCode))
        {
             // Keep only numbers/letters/hyphens/spaces
             var cleanZip = zipCode.RemoveDiacritics();
             ZipCode = cleanZip.Length > 10 ? cleanZip[..10] : cleanZip;
        }
    }
}