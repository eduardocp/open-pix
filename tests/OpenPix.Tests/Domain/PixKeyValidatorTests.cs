using OpenPix.Core.Domain;
using Xunit;

namespace OpenPix.Tests.Domain;

public class PixKeyValidatorTests
{
    [Theory]
    [InlineData("user@example.com", true)]
    [InlineData("invalid-email", false)]
    [InlineData("+5511999999999", true)]
    [InlineData("11999999999", false)]
    [InlineData("123e4567-e89b-12d3-a456-426614174000", true)]
    public void Should_Validate_Simple_Keys(string key, bool expected)
    {
        Assert.Equal(expected, PixKeyValidator.IsValid(key));
    }

    [Theory]
    // CPF
    [InlineData("90442747080", true)] // User Provided Valid
    [InlineData("10889624089", true)] // User Provided Valid
    [InlineData("12345678900", false)] // Invalid Checksum
    [InlineData("11111111111", false)] // All digits equal (Common Validation Trap)
    [InlineData("123", false)] // Length
    public void Should_Validate_Cpf_Correctly(string cpf, bool expected)
    {
        Assert.Equal(expected, PixKeyValidator.IsValid(cpf));
    }

    [Theory]
    // CNPJ
    [InlineData("21487005000160", true)] // User Provided Valid
    [InlineData("47156205000124", true)] // User Provided Valid
    [InlineData("00000000000000", true)] // Special case: Theoretically Valid Checksum, but usually banks block. 
                                         // However, our algo allows if checksum passes. 
                                         // Let's test a definitely INVALID checksum:
    [InlineData("06990590000124", false)] // Invalid Checksum
    public void Should_Validate_Cnpj_Correctly(string cnpj, bool expected)
    {
        Assert.Equal(expected, PixKeyValidator.IsValid(cnpj));
    }
}
