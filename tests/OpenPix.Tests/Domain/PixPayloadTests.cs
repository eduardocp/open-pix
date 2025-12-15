using OpenPix.Core.Domain;
using Xunit;

namespace OpenPix.Tests.Domain;

public class PixPayloadTests
{
    [Fact]
    public void Constructor_Should_Store_RawString()
    {
        var raw = "00020126580014BR.GOV.BCB.PIX...";
        var payload = new PixPayload(raw);
        
        Assert.Equal(raw, payload.RawString);
    }
}
