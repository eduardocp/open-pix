using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OpenPix.Core;
using OpenPix.QRCode;

namespace OpenPix.AspNetCore;

public static class OpenPixEndpointRouteBuilderExtensions
{
    /// <summary>
    /// Maps a GET endpoint that generates a Pix QR Code image (PNG).
    /// Query params: amount (decimal), txid (string, optional).
    /// </summary>
    /// <param name="endpoints">The endpoint builder.</param>
    /// <param name="pattern">The route pattern (default: "/pix/qrcode").</param>
    public static IEndpointConventionBuilder MapPixQrCode(this IEndpointRouteBuilder endpoints, string pattern = "/pix/qrcode")
    {
        return endpoints.MapGet(pattern, (decimal amount, string? txId, IPixClient pixClient) =>
        {
            if (amount <= 0) return Results.BadRequest("Amount must be greater than zero.");

            try
            {
                var payload = pixClient.CreatePayload(amount, txId);
                
                // We use the Extension method directly from payload string (if we had access to string)
                // But PixPayload has the RawString.
                // We need OpenPix.QRCode referenced here.
                
                var pngBytes = payload.ToPngBytes(pixelsPerModule: 20);
                return Results.File(pngBytes, "image/png");
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });
    }
}
