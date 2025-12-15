using System.Globalization;
using OpenPix.Core.Domain;
using OpenPix.Core.Infra;

namespace OpenPix.Core;

public static class PixParser
{
    // Tag IDs according to BACEN manual
    private const string IdMerchantAccount = "26";
    private const string IdAmount = "54";
    private const string IdMerchantName = "59";
    private const string IdMerchantCity = "60";
    private const string IdAdditionalData = "62";
    private const string IdCrc16 = "63";

    public static PixPayload Parse(string pixString)
    {
        if (string.IsNullOrWhiteSpace(pixString))
            throw new ArgumentNullException(nameof(pixString));

        // 1. Integrity Validation (CRC)
        // The CRC is always the last 4 characters
        if (pixString.Length < 4)
            throw new ArgumentException("String PIX muito curta/inválida.");

        var dataWithoutCrc = pixString[..^4];
        var providedCrc = pixString[^4..];
        var calculatedCrc = Crc16.ComputeChecksum(dataWithoutCrc);

        if (!providedCrc.Equals(calculatedCrc, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException($"CRC Inválido. Esperado: {calculatedCrc}, Recebido: {providedCrc}");
        }

        // 2. Data Extraction
        // We use Span to traverse the string without creating unnecessary memory copies
        var span = pixString.AsSpan();

        string? name = null;
        string? city = null;
        string? key = null;
        string? url = null;
        decimal? amount = null;
        string? txId = null;

        int i = 0;
        while (i < span.Length)
        {
            // Protection against reading past the end
            if (i + 4 > span.Length) break;

            var id = span.Slice(i, 2);
            var lenStr = span.Slice(i + 2, 2);

            if (!int.TryParse(lenStr, out int length)) break;

            i += 4;
            if (i + length > span.Length) break;

            var value = span.Slice(i, length);

            // Routing
            if (id.SequenceEqual(IdMerchantName)) name = value.ToString();
            else if (id.SequenceEqual(IdMerchantCity)) city = value.ToString();
            else if (id.SequenceEqual(IdAmount))
            {
                if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal val))
                    amount = val;
            }
            else if (id.SequenceEqual(IdMerchantAccount))
            {
                // Tries to read the Key (01)
                key = ExtractSubField(value, "01");
                url = ExtractSubField(value, "25");
            }
            else if (id.SequenceEqual(IdAdditionalData))
            {
                // TxId is inside tag 62, subtag 05
                txId = ExtractSubField(value, "05");
            }

            i += length;
        }

        // 3. Domain Object Construction
        return new PixPayload(pixString)
        {
            PixKey = key,
            Url = url,
            Merchant = (name != null && city != null) ? new Merchant(name, city) : null,
            Amount = amount,
            TxId = new TransactionId(txId)
        };
    }

    // Helper method to read sub-fields (e.g., Key inside tag 26)
    private static string? ExtractSubField(ReadOnlySpan<char> container, ReadOnlySpan<char> targetId)
    {
        int i = 0;
        while (i < container.Length)
        {
            if (i + 4 > container.Length) break;
            var id = container.Slice(i, 2);
            var len = int.Parse(container.Slice(i + 2, 2));

            if (id.SequenceEqual(targetId))
            {
                return container.Slice(i + 4, len).ToString();
            }
            i += 4 + len;
        }
        return null;
    }
}