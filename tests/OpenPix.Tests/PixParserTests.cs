using OpenPix.Core;
using Xunit;

namespace OpenPix.Tests;

public class PixParserTests
{
    [Fact]
    public void Should_Throw_Exception_When_Crc_Is_Invalid()
    {
        // Arrange
        var validPix = "00020126360014br.gov.bcb.pix0114123456789005204000053039865802BR5913Loja Clean Code6009Sao Paulo62070503***6304C28C";
        var corruptedPix = validPix[..^1] + "D";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => PixParser.Parse(corruptedPix));
        Assert.Contains("CRC Inválido", ex.Message);
    }

    [Fact]
    public void Should_Parse_Static_Pix_Correctly()
    {
        var payload = PixBuilder.Create()
            .WithKey("teste@pix.com.br")
            .WithMerchant("Testador", "Cidade")
            .Build();

        var result = PixParser.Parse(payload);

        Assert.Equal("teste@pix.com.br", result.PixKey);
        Assert.Equal("Testador", result.Merchant?.Name);
    }

    [Fact]
    public void Should_Parse_Pix_With_ZipCode()
    {
        var zipCode = "12345678";
        var payload = PixBuilder.Create()
            .WithKey("chave@pix")
            .WithMerchant("Loja", "Cidade", zipCode)
            .Build();

        var result = PixParser.Parse(payload);

        Assert.NotNull(result.Merchant);
        Assert.Equal(zipCode, result.Merchant.ZipCode);
    }

    [Fact]
    public void Should_Parse_Dynamic_Pix_Url()
    {
        // Arrange
        var url = "https://qr.banco.com/uuid";
        var rawPix = PixBuilder.Create()
            .WithDynamicUrl(url)
            .WithMerchant("Loja Teste", "SP")
            .WithTransactionId("***")
            .Build();

        // Act
        var result = PixParser.Parse(rawPix);

        // Assert
        Assert.Equal(url, result.Url);
        Assert.Null(result.PixKey);
    }
    [Fact]
    public void Should_Throw_Exception_If_PixString_Is_Null_Or_WhiteSpace()
    {
        Assert.Throws<ArgumentNullException>(() => PixParser.Parse(null!));
        Assert.Throws<ArgumentNullException>(() => PixParser.Parse(""));
        Assert.Throws<ArgumentNullException>(() => PixParser.Parse("   "));
    }

    [Fact]
    public void Should_Throw_Exception_If_PixString_Is_Too_Short()
    {
        Assert.Throws<ArgumentException>(() => PixParser.Parse("123"));
    }

    [Fact]
    public void Should_Return_Null_Merchant_If_Tags_Missing()
    {
        var rawWithoutCrc = "0002016304";
        var crc = OpenPix.Core.Infra.Crc16.ComputeChecksum(rawWithoutCrc);
        var pix = rawWithoutCrc + crc;

        var result = PixParser.Parse(pix);

        Assert.Null(result.Merchant);
        Assert.NotNull(result.RawString);
    }
}