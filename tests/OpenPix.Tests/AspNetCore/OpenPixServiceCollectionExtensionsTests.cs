using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenPix.AspNetCore;
using Xunit;

namespace OpenPix.Tests.AspNetCore;

public class OpenPixServiceCollectionExtensionsTests
{
    [Fact]
    public void AddOpenPix_Should_Register_Services()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddOpenPix(options =>
        {
            options.PixKey = "key";
        });

        var provider = services.BuildServiceProvider();

        // Assert
        var client = provider.GetService<IPixClient>();
        Assert.NotNull(client);

        var options = provider.GetService<IOptions<OpenPixOptions>>();
        Assert.NotNull(options);
        Assert.Equal("key", options.Value.PixKey);
    }
}
