using OpenPix.Core.Domain;
using Xunit; // Certifique-se que o xUnit está instalado

namespace OpenPix.Tests;

public class MerchantTests
{
    [Fact]
    public void Should_Remove_Diacritics_From_Name_And_City()
    {
        // Arrange
        var name = "João da Silva";
        var city = "São Paulo";

        // Act
        var merchant = new Merchant(name, city);

        // Assert
        Assert.Equal("Joao da Silva", merchant.Name);
        Assert.Equal("Sao Paulo", merchant.City);
    }

    [Fact]
    public void Should_Truncate_Name_If_Too_Long()
    {
        // Arrange (Nome com 30 chars)
        var longName = "Empresa Muito Grande LTDA do Brasil";

        // Act
        var merchant = new Merchant(longName, "SP");

        // Assert (Deve cortar no 25º caractere)
        Assert.Equal(25, merchant.Name.Length);
        Assert.Equal("Empresa Muito Grande LTDA", merchant.Name);
    }

    [Fact]
    public void Should_Truncate_City_If_Too_Long()
    {
        // Arrange (Cidade com mais de 15 chars)
        var longCity = "Pindamonhangaba do Norte"; // 24 chars

        // Act
        var merchant = new Merchant("Loja", longCity);

        // Assert (Deve cortar no 15º caractere)
        Assert.Equal(15, merchant.City.Length);
        Assert.Equal("Pindamonhangaba", merchant.City);
    }
}