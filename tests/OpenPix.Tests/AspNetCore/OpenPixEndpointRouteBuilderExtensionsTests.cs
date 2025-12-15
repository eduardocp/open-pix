using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenPix.AspNetCore;
using System.Net;

namespace OpenPix.Tests.AspNetCore;

public class OpenPixEndpointRouteBuilderExtensionsTests
{
    [Fact]
    public async Task MapPixQrCode_Should_Return_Png_Image_When_Valid_Request()
    {
        // Arrange
        using var host = await new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddRouting();
                        // Register OpenPix
                        services.AddOpenPix(options =>
                        {
                            options.PixKey = "test@pix.com";
                            options.MerchantName = "Teste";
                            options.City = "SP";
                        });
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapPixQrCode("/pix/test");
                        });
                    });
            })
            .StartAsync();

        var client = host.GetTestClient();

        // Act
        var response = await client.GetAsync("/pix/test?amount=10.00&txid=TEST1234");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("image/png", response.Content.Headers.ContentType?.ToString());
        
        var bytes = await response.Content.ReadAsByteArrayAsync();
        Assert.True(bytes.Length > 0);
    }

    [Fact]
    public async Task MapPixQrCode_Should_Return_BadRequest_If_Amount_Is_Zero_Or_Negative()
    {
        // Arrange
        using var host = await new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddRouting();
                        services.AddOpenPix(options =>
                        {
                            options.PixKey = "test@pix.com";
                            options.MerchantName = "Teste";
                            options.City = "SP";
                        });
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapPixQrCode("/pix/test");
                        });
                    });
            })
            .StartAsync();

        var client = host.GetTestClient();

        // Act
        var response = await client.GetAsync("/pix/test?amount=0");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Amount must be greater than zero", content);
    }
    
    [Fact, Trait("Description", "Tests the implicit catch block")]
    public async Task MapPixQrCode_Should_Return_Problem_On_Exception()
    {
         // Arrange
        using var host = await new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddRouting();
                        // Misconfigured PixClient (missing services or bad config)
                        // Actually, AddOpenPix validates on construction usually, 
                        // but let's try to inject a mock IPixClient that throws
                        services.AddSingleton<IPixClient>(new MockThrowingPixClient());
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapPixQrCode("/pix/bug");
                        });
                    });
            })
            .StartAsync();

        var client = host.GetTestClient();

        // Act
        var response = await client.GetAsync("/pix/bug?amount=10");

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    private class MockThrowingPixClient : IPixClient
    {
        public OpenPix.Core.Domain.PixPayload CreatePayload(decimal amount, string? txId = null)
        {
            throw new InvalidOperationException("Simulated Failure");
        }

        public OpenPix.Core.Domain.PixPayload CreatePayload(string pixKey, decimal amount, string? txId = null)
        {
            throw new NotImplementedException();
        }
    }
}
