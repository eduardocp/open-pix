using System.CommandLine;
using OpenPix.Core;
using OpenPix.QRCode;

namespace OpenPix.Cli;

public static class CliCommands
{
    public static RootCommand BuildRootCommand()
    {
        var rootCommand = new RootCommand("OpenPix CLI - Ferramenta para gerar e ler códigos Pix");

        // --- GENERATE COMMAND ---
        var genCommand = new Command("gen", "Gera um QR Code Pix estático ou dinâmico");

        var keyOption = new Option<string?>("--key", "Chave Pix (CPF, Email, Tel, Aleatória)");
        var urlOption = new Option<string?>("--url", "URL do PSP para Pix Dinâmico");
        var nameOption = new Option<string>("--name", "Nome do Recebedor") { IsRequired = true };
        var cityOption = new Option<string>("--city", "Cidade do Recebedor") { IsRequired = true };
        var zipOption = new Option<string?>("--zip", "CEP do Recebedor");
        var amountOption = new Option<decimal?>("--amount", "Valor da cobrança");
        var txIdOption = new Option<string?>("--txid", "Identificador da Transação (Padrão: ***)");

        genCommand.AddOption(keyOption);
        genCommand.AddOption(urlOption);
        genCommand.AddOption(nameOption);
        genCommand.AddOption(cityOption);
        genCommand.AddOption(zipOption);
        genCommand.AddOption(amountOption);
        genCommand.AddOption(txIdOption);

        genCommand.SetHandler((key, url, name, city, zip, amount, txId) =>
        {
            var builder = PixBuilder.Create()
                .WithMerchant(name, city, zip);

            if (!string.IsNullOrEmpty(key)) builder.WithKey(key);
            else if (!string.IsNullOrEmpty(url)) builder.WithDynamicUrl(url);
            else 
            {
                throw new ArgumentException("Erro: Você deve fornecer --key OU --url.");
            }

            if (amount.HasValue) builder.WithAmount(amount.Value);
            if (!string.IsNullOrEmpty(txId)) builder.WithTransactionId(txId);

            var payloadStr = builder.Build();
            var payloadObj = PixParser.Parse(payloadStr); 

            Console.WriteLine("\n=== PIX GERADO ===");
            Console.WriteLine(payloadStr);
            Console.WriteLine("\n=== QR CODE ===");
            
            Console.WriteLine(payloadObj.ToAsciiArt(small: true));

        }, keyOption, urlOption, nameOption, cityOption, zipOption, amountOption, txIdOption);

        // --- DECODE COMMAND ---
        var decodeCommand = new Command("decode", "Lê e valida uma string Pix");
        var inputArgument = new Argument<string>("pix-string", "A string Pix (copia e cola)");

        decodeCommand.AddArgument(inputArgument);

        decodeCommand.SetHandler((pixString) =>
        {
            var data = PixParser.Parse(pixString);
            Console.WriteLine("\n=== DADOS DO PIX ===");
            Console.WriteLine($"Nome:      {data.Merchant?.Name}");
            Console.WriteLine($"Cidade:    {data.Merchant?.City}");
            if(!string.IsNullOrEmpty(data.Merchant?.ZipCode)) 
                Console.WriteLine($"CEP:       {data.Merchant.ZipCode}");
                
            Console.WriteLine($"Valor:     {(data.Amount.HasValue ? data.Amount.Value.ToString("C2") : "Não informado")}");
            
            if (!string.IsNullOrEmpty(data.Url))
                    Console.WriteLine($"URL P.S.P: {data.Url}");
            else
                    Console.WriteLine($"Chave Pix: {data.PixKey}");
                    
            Console.WriteLine($"TxID:      {data.TxId.Value}");
            Console.WriteLine("Checksum:  OK (Válido)");

        }, inputArgument);

        rootCommand.AddCommand(genCommand);
        rootCommand.AddCommand(decodeCommand);

        return rootCommand;
    }
}
