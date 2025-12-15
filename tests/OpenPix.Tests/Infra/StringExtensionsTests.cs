using OpenPix.Core.Infra;
using Xunit;

namespace OpenPix.Tests.Infra;

public class StringExtensionsTests
{
    [Fact]
    public void RemoveDiacritics_Should_Remove_Accents()
    {
        var input = "Atenção, João!";
        var expected = "Atencao, Joao!";
        
        var result = input.RemoveDiacritics();
        
        Assert.Equal(expected, result);
    }

    [Fact]
    public void RemoveDiacritics_Should_Handle_Empty_String()
    {
        var result = "".RemoveDiacritics();
        Assert.Equal("", result);
    }

    [Fact]
    public void RemoveDiacritics_Should_Return_Same_String_If_No_Accents()
    {
        var input = "OpenPix";
        var result = input.RemoveDiacritics();
        Assert.Equal(input, result);
    }
}
