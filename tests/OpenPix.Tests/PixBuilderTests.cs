using OpenPix.Core;
using Xunit;

namespace OpenPix.Tests;

public class PixBuilderTests
{
    [Fact]
    public void Should_Build_Dynamic_Pix_With_Url()
    {
        // Arrange
        var url = "https://pix.example.com/qr/v2/9d36b84f-70b3-40a1";

        // Act
        var payload = PixBuilder.Create()
            .WithDynamicUrl(url)
            .WithMerchant("Loja Dinamica", "Sao Paulo")
            .WithAmount(100.00m)
            .WithTransactionId("PEDIDODINAMICO")
            .Build();

        // Assert
        Assert.NotNull(payload);
        Assert.Contains(url, payload);
    }

    [Fact]
    public void Should_Include_ZipCode_In_Payload()
    {
        // Arrange
        var zipCode = "12345678";
        
        // Act
        var payload = PixBuilder.Create()
            .WithKey("test@key")
            .WithMerchant("Loja", "Sao Paulo", zipCode)
            .WithAmount(10.00m)
            .Build();

        // Assert
        // Tag 61 (Postal Code) should be present
        // 61 -> Tag ID
        // 08 -> Length
        // 12345678 -> Value
        Assert.Contains("610812345678", payload);
    }

    [Fact]
    public void Should_Throw_If_Both_Key_And_Url_Are_Provided()
    {
        // Arrange
        var builder = PixBuilder.Create()
            .WithKey("chave@pix.com")
            .WithDynamicUrl("https://url-do-banco.com")
            .WithMerchant("Teste", "Cidade");

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => builder.Build());

        Assert.Contains("WithKey", ex.Message);
        Assert.Contains("WithDynamicUrl", ex.Message);
    }

    [Fact]
    public void Should_Throw_If_Url_Is_Not_Https()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            PixBuilder.Create().WithDynamicUrl("http://inseguro.com")
        );
    }
    
    [Fact]
    public void Should_Throw_If_Amount_Is_Negative_Or_Zero()
    {
         Assert.Throws<ArgumentOutOfRangeException>(() => PixBuilder.Create().WithAmount(-1));
         Assert.Throws<ArgumentOutOfRangeException>(() => PixBuilder.Create().WithAmount(0));
    }

    [Fact]
    public void Should_Throw_If_No_Merchant_Is_Provided()
    {
        var builder = PixBuilder.Create()
            .WithKey("test@key");
            
        var ex = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.Contains("Merchant info is required", ex.Message);
    }
    
    [Fact]
    public void Should_Throw_If_No_Key_And_No_Url()
    {
        var builder = PixBuilder.Create()
            .WithMerchant("Loja", "Cidade");
            
        var ex = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.Contains("Configure uma Chave", ex.Message);
    }

    [Fact]
    public void Should_Throw_If_Url_Is_Null_Or_Empty()
    {
        Assert.Throws<ArgumentNullException>(() => PixBuilder.Create().WithDynamicUrl(null!));
        Assert.Throws<ArgumentNullException>(() => PixBuilder.Create().WithDynamicUrl(""));
    }
}