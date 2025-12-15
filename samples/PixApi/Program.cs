using OpenPix.AspNetCore;
using OpenPix.QRCode; // Para usar o .ToPngBase64()

var builder = WebApplication.CreateBuilder(args);

// === CONFIGURAÇÃO (O que o usuário vai fazer) ===
builder.Services.AddOpenPix(options =>
{
    options.PixKey = "12548656756";
    options.MerchantName = "Mega Loja";
    options.City = "Curitiba";
});
// ===============================================

var app = builder.Build();

app.MapGet("/", () => "API de PIX rodando! Acesse /qrcode?valor=10");

// Endpoint que gera o PIX
app.MapGet("/qrcode", (decimal? valor, IPixClient pixClient) =>
{
    if (!valor.HasValue) return Results.BadRequest("Informe o valor. Ex: /qrcode?valor=12.50");

    // O cliente já sabe a chave e o nome da loja
    var payload = pixClient.CreatePayload(valor.Value, txId: "API001");

    // Gera a imagem
    var base64 = payload.RawString.ToPngBase64();

    // Retorna HTML para facilitar a visualização
    var html = $@"
    <html>
        <body style='display:flex; justify-content:center; align-items:center; height:30vh; background:#eee;'>
            <div style='background:white; padding:20px; text-align:center; border-radius:10px;'>
                <h1>Pague {valor:C}</h1>
                <img src='data:image/png;base64,{base64}' style='height:60%;' />
                <p><code>{payload.RawString}</code></p>
            </div>
        </body>
    </html>";

    return Results.Content(html, "text/html");
});

app.Run();