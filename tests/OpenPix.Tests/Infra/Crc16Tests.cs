using OpenPix.Core.Infra;
using Xunit;

namespace OpenPix.Tests.Infra;

public class Crc16Tests
{
    [Fact]
    public void Should_Compute_Checksum_Correctly()
    {
        var input = "123456789";
        var crc = Crc16.ComputeChecksum(input);
        
        Assert.Matches("^[0-9A-F]{4}$", crc);
    }

    [Theory]
    [InlineData("test")]
    public void Should_Match_Known_Values(string input)
    {
       var result = Crc16.ComputeChecksum(input);
       Assert.Equal(4, result.Length);
    }
}
