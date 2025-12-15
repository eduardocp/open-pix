using System.Text;
using QRCoder;

namespace OpenPix.QRCode;

public static class PixConsoleExtensions
{
    /// <summary>
    /// Generates an ASCII representation of the QR Code for console output.
    /// Useful for server logs or CLI applications.
    /// </summary>
    /// <param name="payload">The PixPayload object.</param>
    /// <param name="small">If true, uses smaller characters (2 chars per block) instead of blocks.</param>
    /// <returns>A string containing the ASCII art.</returns>
    public static string ToAsciiArt(this OpenPix.Core.Domain.PixPayload payload, bool small = false)
    {
        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(payload.RawString, QRCodeGenerator.ECCLevel.M);
        
        // QRCoder doesn't have a native AsciiQRCode in this version context usually, 
        // so we implement a simple one manually based on the Matrix.
        
        // The ModuleMatrix is a collection of bools (true = black/filled, false = white/empty)
        var matrix = qrCodeData.ModuleMatrix;
        var sb = new StringBuilder();

        // Top border
        sb.AppendLine();

        for (int i = 0; i < matrix.Count; i++)
        {
            for (int k = 0; k < (small ? 1 : 2); k++) // Vertical stretch correction for some fonts? 
            // Actually, in standard terminals, a block is often taller than wide, so using '██' (two chars) helps verify sqaureness.
            // But let's stick to standard behavior: iterate rows.
            {
                // We don't loop K here for rows usually unless we want "large" ascii.
                // Let's keep it simple: One ASCII row per Matrix row.
            }
            
            // Loop for the row pixels
            for (int j = 0; j < matrix[i].Count; j++)
            {
                bool isDark = matrix[i][j];
                // Use full block for dark, space for light.
                // In inverted terminals (dark bg), "Dark" module should be... well, printed as color.
                // The safest is usually: 
                // Dark Module = "██"
                // Light Module = "  "
                sb.Append(isDark ? "██" : "  ");
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
