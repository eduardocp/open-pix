using System.Globalization;
using System.Text;

namespace OpenPix.Core.Infra;

internal static class EmvCodec
{
    // Formata um campo TLV simples: "00" + "02" + "01"
    public static string Format(string id, string value)
    {
        return $"{id}{value.Length:D2}{value}";
    }

    // Formata o valor monetário no padrão PIX (ex: 1.50)
    public static string FormatAmount(decimal amount)
    {
        return amount.ToString("F2", CultureInfo.InvariantCulture);
    }

    // Monta o payload final e assina com CRC
    public static string AssemblePayload(StringBuilder rawPayload)
    {
        // Adiciona o ID do CRC (63) e o tamanho (04)
        rawPayload.Append("6304");

        // Calcula
        var crc = Crc16.ComputeChecksum(rawPayload.ToString());

        // Anexa
        rawPayload.Append(crc);

        return rawPayload.ToString();
    }
}