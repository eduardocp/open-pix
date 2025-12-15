using QRCoder;

namespace OpenPix.QRCode;

public static class PixImageExtensions
{
    /// <summary>
    /// Generates the PIX graphic representation in Base64 (PNG).
    /// Ideal for displaying in <img src="data:image/png;base64,..." /> tags
    /// </summary>
    public static byte[] ToPngBytes(this OpenPix.Core.Domain.PixPayload payload, int pixelsPerModule = 20)
    {
        return payload.RawString.ToPngBytes(pixelsPerModule);
    }

    public static byte[] ToPngBytes(this string pixString, int pixelsPerModule = 20)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(pixString, QRCodeGenerator.ECCLevel.M);
        using var qrCode = new PngByteQRCode(qrCodeData);
        return qrCode.GetGraphic(pixelsPerModule);
    }

    public static string ToPngBase64(this OpenPix.Core.Domain.PixPayload payload, int pixelsPerModule = 20)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(payload.RawString, QRCodeGenerator.ECCLevel.M);

        // We use PngByteQRCode as it does not depend on System.Drawing (works on Linux/Docker)
        using var qrCode = new PngByteQRCode(qrCodeData);

        byte[] qrCodeBytes = qrCode.GetGraphic(pixelsPerModule);
        return Convert.ToBase64String(qrCodeBytes);
    }

    /// <summary>
    /// Generates the PIX graphic representation in Base64 (PNG).
    /// Ideal for displaying in <img src="data:image/png;base64,..." /> tags
    /// </summary>
    public static string ToPngBase64(this string pixString, int pixelsPerModule = 20)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(pixString, QRCodeGenerator.ECCLevel.M);

        // We use PngByteQRCode as it does not depend on System.Drawing (works on Linux/Docker)
        using var qrCode = new PngByteQRCode(qrCodeData);

        byte[] qrCodeBytes = qrCode.GetGraphic(pixelsPerModule);
        return Convert.ToBase64String(qrCodeBytes);
    }

    /// <summary>
    /// Generates the QR Code SVG code.
    /// Ideal for infinite scaling without losing quality.
    /// </summary>
    public static string ToSvg(this string pixString, int pixelsPerModule = 20)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(pixString, QRCodeGenerator.ECCLevel.M);

        using var qrCode = new SvgQRCode(qrCodeData);
        return qrCode.GetGraphic(pixelsPerModule);
    }
}