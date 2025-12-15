using System.CommandLine;
using System.CommandLine.Parsing;
using Xunit;
using OpenPix.Core; // Import this

namespace OpenPix.Cli.Tests;

public class CliTests
{
    private readonly Parser _parser;

    public CliTests()
    {
        var rootCommand = CliCommands.BuildRootCommand();
        _parser = new Parser(rootCommand);
    }

    [Fact]
    public async Task Should_Fail_If_Required_Options_Are_Missing()
    {
        // Act & Assert
        // Since we removed the try/catch in the handler, the internal logic (Merchant constructor)
        // throws ArgumentNullException because Name is null.
        // System.CommandLine propagates this exception.
        await Assert.ThrowsAnyAsync<Exception>(async () => 
        {
             await _parser.InvokeAsync("gen");
        });
    }

    [Fact]
    public async Task Should_Generate_Pix_Successfully()
    {
        // Arrange
        var args = "gen --name Teste --city SP --key teste@pix.com --amount 1.00";

        // Act
        var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        var result = await _parser.InvokeAsync(args);

        // Restore console
        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);

        // Assert
        Assert.Equal(0, result);
        Assert.Contains("=== PIX GERADO ===", consoleOutput.ToString());
        Assert.Contains("=== QR CODE ===", consoleOutput.ToString());
    }

    [Fact]
    public async Task Should_Decode_Pix_Successfully()
    {
        // 1. Generate a VALID Pix on the fly to avoid CRC mismatch
        var validPix = PixBuilder.Create()
            .WithKey("teste@pix.com")
            .WithMerchant("Loja Teste", "Sao Paulo")
            .WithTransactionId("***")
            .Build();
        
        // Act
        var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // Wrap payload in quotes to ensure it's treated as a single argument if it contains spaces (though pix usually doesn't)
        // However, standard pix strings don't have spaces.
        // The error 'Recebido: Loja' is weird. Pix strings end with CRC like '6304ABCD'.
        // If the 'validPix' string is somehow malformed/contains 'Loja' at the end? 
        // Wait, PixBuilder output is clean.
        // Let's debug by printing or just being safe with quotes.
        var result = await _parser.InvokeAsync(new string[] { "decode", validPix });

        // Restore console
        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);

        // Assert
        Assert.Equal(0, result);
        Assert.Contains("=== DADOS DO PIX ===", consoleOutput.ToString());
        Assert.Contains("Nome:      Loja Teste", consoleOutput.ToString()); // Check for the name we put
        Assert.Contains("Checksum:  OK (Válido)", consoleOutput.ToString());
    }
}
