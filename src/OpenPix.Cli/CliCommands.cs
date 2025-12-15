using System.CommandLine;
using OpenPix.Core;
using OpenPix.QRCode;

namespace OpenPix.Cli;

public static class CliCommands
{
    public static RootCommand BuildRootCommand()
    {
        var rootCommand = new RootCommand("OpenPix CLI - Tool for generating and decoding Pix codes");

        // --- GENERATE COMMAND ---
        var genCommand = new Command("gen", "Generates a Static or Dynamic Pix QR Code");

        var keyOption = new Option<string?>("--key", "Pix Key (CPF, Email, Phone, Random)");
        var urlOption = new Option<string?>("--url", "PSP URL for Dynamic Pix");
        var nameOption = new Option<string>("--name", "Receiver Name") { IsRequired = true };
        var cityOption = new Option<string>("--city", "Receiver City") { IsRequired = true };
        var zipOption = new Option<string?>("--zip", "Receiver Zip Code");
        var amountOption = new Option<decimal?>("--amount", "Charge Amount");
        var txIdOption = new Option<string?>("--txid", "Transaction ID (Default: ***)");

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
                throw new ArgumentException("Error: You must provide --key OR --url.");
            }

            if (amount.HasValue) builder.WithAmount(amount.Value);
            if (!string.IsNullOrEmpty(txId)) builder.WithTransactionId(txId);

            var payloadStr = builder.Build();
            var payloadObj = PixParser.Parse(payloadStr); 

            Console.WriteLine("\n=== GENERATED PIX ===");
            Console.WriteLine(payloadStr);
            Console.WriteLine("\n=== QR CODE ===");
            
            Console.WriteLine(payloadObj.ToAsciiArt(small: true));

        }, keyOption, urlOption, nameOption, cityOption, zipOption, amountOption, txIdOption);

        // --- DECODE COMMAND ---
        var decodeCommand = new Command("decode", "Decodes and validates a Pix string");
        var inputArgument = new Argument<string>("pix-string", "The Pix string to decode");

        decodeCommand.AddArgument(inputArgument);

        decodeCommand.SetHandler((pixString) =>
        {
            var data = PixParser.Parse(pixString);
            Console.WriteLine("\n=== PIX DATA ===");
            Console.WriteLine($"Name:      {data.Merchant?.Name}");
            Console.WriteLine($"City:      {data.Merchant?.City}");
            if(!string.IsNullOrEmpty(data.Merchant?.ZipCode)) 
                Console.WriteLine($"Zip:       {data.Merchant.ZipCode}");
                
            Console.WriteLine($"Amount:    {(data.Amount.HasValue ? data.Amount.Value.ToString("C2") : "Not specified")}");
            
            if (!string.IsNullOrEmpty(data.Url))
                    Console.WriteLine($"PSP URL:   {data.Url}");
            else
                    Console.WriteLine($"Pix Key:   {data.PixKey}");
                    
            Console.WriteLine($"TxID:      {data.TxId.Value}");
            Console.WriteLine("Checksum:  OK (Valid)");

        }, inputArgument);

        rootCommand.AddCommand(genCommand);
        rootCommand.AddCommand(decodeCommand);

        return rootCommand;
    }
}
