using QRCoder;

namespace OpenPix.QRCode;

public static class PixImageExtensions
{
    /// <summary>
    /// Gera a representação gráfica do PIX em formato Base64 (PNG).
    /// Ideal para exibir em tags <img src="data:image/png;base64,..." />
    /// </summary>
    public static string ToPngBase64(this string pixString, int pixelsPerModule = 20)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(pixString, QRCodeGenerator.ECCLevel.M);

        // Usamos PngByteQRCode pois ele não depende de System.Drawing (funciona no Linux/Docker)
        using var qrCode = new PngByteQRCode(qrCodeData);

        byte[] qrCodeBytes = qrCode.GetGraphic(pixelsPerModule);
        return Convert.ToBase64String(qrCodeBytes);
    }

    /// <summary>
    /// Gera o código SVG do QR Code.
    /// Ideal para escalar infinitamente sem perder qualidade.
    /// </summary>
    public static string ToSvg(this string pixString, int pixelsPerModule = 20)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(pixString, QRCodeGenerator.ECCLevel.M);

        using var qrCode = new SvgQRCode(qrCodeData);
        return qrCode.GetGraphic(pixelsPerModule);
    }
}