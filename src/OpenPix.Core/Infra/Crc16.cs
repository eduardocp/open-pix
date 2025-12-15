using System.Text;

namespace OpenPix.Core.Infra;

internal static class Crc16
{
    private const ushort Polynomial = 0x1021;
    private const ushort InitialValue = 0xFFFF;

    public static string ComputeChecksum(string payload)
    {
        // Importante: PIX trabalha com ASCII estendido/UTF8 simples. 
        // Vamos garantir que estamos pegando os bytes corretos.
        var bytes = Encoding.UTF8.GetBytes(payload);

        ushort crc = InitialValue;

        foreach (byte b in bytes)
        {
            for (int i = 0; i < 8; i++)
            {
                bool bit = ((b >> (7 - i) & 1) == 1);
                bool c15 = ((crc >> 15 & 1) == 1);
                crc <<= 1;
                if (c15 ^ bit) crc ^= Polynomial;
            }
        }

        return crc.ToString("X4");
    }
}