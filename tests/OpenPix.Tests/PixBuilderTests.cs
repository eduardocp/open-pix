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
        Assert.Contains(url, payload); // Garante que a URL entrou na string
    }

    [Fact]
    public void Should_Throw_If_Both_Key_And_Url_Are_Provided()
    {
        // Arrange
        var builder = PixBuilder.Create()
            .WithKey("chave@pix.com")
            .WithDynamicUrl("https://url-do-banco.com") // <--- Conflito!
            .WithMerchant("Teste", "Cidade");

        // Act & Assert
        // O Pix não pode ser Estático e Dinâmico ao mesmo tempo
        // Verifique se a sua implementação do ValidateState lança InvalidOperationException
        var ex = Assert.Throws<InvalidOperationException>(() => builder.Build());

        Assert.Contains("WithKey", ex.Message); // O erro deve mencionar as opções
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
}