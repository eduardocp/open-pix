using OpenPix.Core.Infra;
using System.Text;
using Xunit;

namespace OpenPix.Tests.Infra;

public class EmvCodecTests
{
    [Fact]
    public void Format_Should_Return_TLV_String()
    {
        var id = "00";
        var value = "01";
        // 00 + length(2) + value
        var expected = "000201";
        
        var result = EmvCodec.Format(id, value);
        
        Assert.Equal(expected, result);
    }

    [Fact]
    public void FormatAmount_Should_Format_To_Two_Decimal_Places()
    {
        decimal amount = 10.5m;
        var expected = "10.50";
        
        var result = EmvCodec.FormatAmount(amount);
        
        Assert.Equal(expected, result);
    }

    [Fact]
    public void FormatAmount_Should_Use_Dot_Separator()
    {
        decimal amount = 1.00m;
        var expected = "1.00";
        
        var result = EmvCodec.FormatAmount(amount);
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void AssemblePayload_Should_Append_Crc()
    {
        // 000201 = Payload fake
        var sb = new StringBuilder("000201");
        
        // AssemblePayload adiciona "6304" e o CRC.
        // Total = 000201 + 6304 + CRC(4 chars)
        var result = EmvCodec.AssemblePayload(sb);
        
        Assert.EndsWith("6304", result.Substring(0, result.Length - 4));
        Assert.Equal(6 + 4 + 4, result.Length);
    }
}
