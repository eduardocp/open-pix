using OpenPix.QRCode;
using Xunit;

namespace OpenPix.Tests.QRCode;

public class PixImageExtensionsTests
{
    [Fact]
    public void ToPngBase64_Should_Return_Valid_Base64_String()
    {
        var pixString = "00020101021226580014BR.GOV.BCB.PIX...";
        
        var base64 = pixString.ToPngBase64();
        
        Assert.NotNull(base64);
        Assert.NotEmpty(base64);
        
        var bytes = Convert.FromBase64String(base64);
        Assert.True(bytes.Length > 0);
        Assert.Equal(0x89, bytes[0]);
        Assert.Equal(0x50, bytes[1]);
        Assert.Equal(0x4E, bytes[2]);
        Assert.Equal(0x47, bytes[3]);
    }

    [Fact]
    public void ToPngBytes_Should_Return_Png_Image_Bytes()
    {
        var pixString = "00020126580014BR.GOV.BCB.PIX0136123e4567-e12b-12d1-a456-426655440000";
        
        var bytes = pixString.ToPngBytes();
        
        Assert.NotNull(bytes);
        Assert.NotEmpty(bytes);
        
        // Validates PNG Magic Number (89 50 4E 47 0D 0A 1A 0A)
        Assert.Equal(0x89, bytes[0]);
        Assert.Equal(0x50, bytes[1]);
        Assert.Equal(0x4E, bytes[2]);
        Assert.Equal(0x47, bytes[3]);
    }

    [Fact]
    public void ToPngBytes_From_Payload_Should_Return_Bytes()
    {
        var payload = new OpenPix.Core.Domain.PixPayload("000201...");
        var bytes = payload.ToPngBytes();
        
        Assert.NotNull(bytes);
        Assert.NotEmpty(bytes);
        Assert.Equal(0x89, bytes[0]); // PNG Header
    }

    [Fact]
    public void ToPngBase64_From_Payload_Should_Return_Valid_Base64()
    {
        var payload = new OpenPix.Core.Domain.PixPayload("000201...");
        var base64 = payload.ToPngBase64();
        
        Assert.NotNull(base64);
        var bytes = Convert.FromBase64String(base64);
        Assert.Equal(0x89, bytes[0]);
    }

    [Fact]
    public void ToSvg_Should_Return_Svg_Content()
    {
        var pixString = "abc";
        
        var svg = pixString.ToSvg();
        
        Assert.NotNull(svg);
        Assert.Contains("<svg", svg);
        Assert.Contains("</svg>", svg);
    }
}
