using System.Globalization;
using System.Text;

namespace OpenPix.Core.Infra;

internal static class EmvCodec
{
    // Formats a simple TLV field: "00" + "02" + "01"
    public static string Format(string id, string value)
    {
        return $"{id}{value.Length:D2}{value}";
    }

    // Formats the monetary value in PIX standard (e.g., 1.50)
    public static string FormatAmount(decimal amount)
    {
        return amount.ToString("F2", CultureInfo.InvariantCulture);
    }

    // Assembles the final payload and signs with CRC
    public static string AssemblePayload(StringBuilder rawPayload)
    {
        // Adds the CRC ID (63) and size (04)
        rawPayload.Append("6304");

        // Calculates
        var crc = Crc16.ComputeChecksum(rawPayload.ToString());

        // Appends
        rawPayload.Append(crc);

        return rawPayload.ToString();
    }
}