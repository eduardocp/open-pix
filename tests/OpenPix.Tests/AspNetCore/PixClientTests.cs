using Microsoft.Extensions.Options;
using OpenPix.AspNetCore;
using Xunit;

namespace OpenPix.Tests.AspNetCore;

public class PixClientTests
{
    [Fact]
    public void Constructor_Should_Throw_If_PixKey_Is_Missing()
    {
        var options = Options.Create(new OpenPixOptions());
        
        var ex = Assert.Throws<InvalidOperationException>(() => new PixClient(options));
        Assert.Contains("PixKey", ex.Message);
    }

    [Fact]
    public void CreatePayload_Should_Use_Options_To_Build_Payload()
    {
        var options = Options.Create(new OpenPixOptions
        {
            PixKey = "test@pix.com",
            MerchantName = "Loja Teste",
            City = "Sao Paulo"
        });

        var client = new PixClient(options);
        var payload = client.CreatePayload(10.00m);

        Assert.NotNull(payload);
        Assert.Equal("Loja Teste", payload.Merchant?.Name);
        Assert.Equal("Sao Paulo", payload.Merchant?.City);
        Assert.Equal("test@pix.com", payload.PixKey);
    }

    [Fact]
    public void CreatePayload_With_Key_Should_Override_Options()
    {
        var options = Options.Create(new OpenPixOptions
        {
            PixKey = "default@pix.com",
            MerchantName = "Loja Teste",
            City = "Sao Paulo"
        });

        var client = new PixClient(options);
        var newKey = "override@pix.com";
        var payload = client.CreatePayload(newKey, 20.00m);

        Assert.NotNull(payload);
    }
}
