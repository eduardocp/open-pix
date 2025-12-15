
using OpenPix.Core;
using OpenPix.QRCode;

Console.WriteLine("=== OpenPix Round-Trip Demo ===\n");

try
{
    // 1. ESCRITA (Builder)
    Console.WriteLine("1. Gerando PIX...");
    var originalPix = PixBuilder.Create()
        .WithKey("12548656756")
        .WithMerchant("Cafeteria C#", "São Paulo")
        .WithAmount(12.50m)
        .WithTransactionId("CAFE1020")
        .Build();

    Console.WriteLine($"[OK] String Gerada: {originalPix}");

    // 2. LEITURA (Parser)
    Console.WriteLine("\n2. Lendo PIX de volta...");
    var parsedData = PixParser.Parse(originalPix);

    // 3. VALIDAÇÃO
    Console.WriteLine("\n--- Dados Extraídos ---");
    Console.WriteLine($"Recebedor: {parsedData.Merchant?.Name}"); // Esperado: CAFETERIA C# (sem acento)
    Console.WriteLine($"Cidade:    {parsedData.Merchant?.City}"); // Esperado: SAO PAULO
    Console.WriteLine($"Valor:     {parsedData.Amount:C}");
    Console.WriteLine($"Chave:     {parsedData.PixKey}");
    Console.WriteLine($"TxID:      {parsedData.TxId.Value}");

    Console.WriteLine("\n[SUCESSO] O PIX gerado foi lido corretamente!");
}
catch (Exception ex)
{
    Console.WriteLine($"[FALHA] {ex.Message}");
}

Console.WriteLine("=== Gerador de QR Code PIX ===\n");

// 1. Gerar a String (Como já fazíamos)
var pixString = PixBuilder.Create()
    .WithKey("12548656756")
    .WithMerchant("Cafeteria C#", "São Paulo")
    .WithAmount(12.50m)
    .WithTransactionId("CAFE1020")
    .Build();

Console.WriteLine($"String PIX: {pixString[..20]}..."); // Mostra só o começo

// 2. Gerar a Imagem (Aqui está a mágica da nova Lib)
// Note que o método .ToPngBase64() aparece na string porque importamos OpenPix.QRCode
string base64Image = pixString.ToPngBase64(pixelsPerModule: 5);

// 3. Salvar um HTML para visualizar
var htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ 
            background-color: #f0f0f0; 
            font-family: sans-serif; 
            display: flex; 
            flex-direction: column; 
            align-items: center; 
            justify-content: center; 
            height: 100vh; 
        }}
        .card {{
            background: white;
            padding: 40px;
            border-radius: 10px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
            text-align: center;
        }}
        img {{
            margin: 20px 0;
            border: 1px solid #ddd; /* Borda fina pra ajudar no contraste visual */
        }}
    </style>
</head>
<body>
    <div class='card'>
        <h1>Pague com PIX</h1>
        <p>Abra o app do seu banco e aponte a câmera:</p>
        
        <img src='data:image/png;base64,{base64Image}' alt='QR Code PIX' />
        
        <p style='font-size: 12px; color: #666;'>Se não funcionar, copie o código abaixo:</p>
        <textarea rows='4' cols='50' style='width:100%'>{pixString}</textarea>
    </div>
</body>
</html>";

string filePath = Path.Combine(Directory.GetCurrentDirectory(), "pix_teste.html");
File.WriteAllText(filePath, htmlContent);

Console.WriteLine($"\n[SUCESSO] Arquivo gerado em: {filePath}");
Console.WriteLine("Abra esse arquivo no navegador para ver o QR Code!");