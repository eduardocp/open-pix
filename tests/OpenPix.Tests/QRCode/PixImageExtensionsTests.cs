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
    public void ToSvg_Should_Return_Svg_Content()
    {
        var pixString = "abc";
        
        var svg = pixString.ToSvg();
        
        Assert.NotNull(svg);
        Assert.Contains("<svg", svg);
        Assert.Contains("</svg>", svg);
    }
}
