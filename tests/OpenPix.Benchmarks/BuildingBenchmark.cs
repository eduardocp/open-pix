using BenchmarkDotNet.Attributes;
using OpenPix.Core;
using System.Text;

namespace OpenPix.Benchmarks;

[MemoryDiagnoser]
public class BuildingBenchmark
{
    [Benchmark(Baseline = true)]
    public string OpenPixBuilder()
    {
        // Fluent API with validation
        return PixBuilder.Create()
            .WithKey("test@benchmark.com")
            .WithMerchant("Loja Benchmark", "Sao Paulo")
            .WithAmount(123.45m)
            .WithTransactionId("BENCH01")
            .Build();
    }

    [Benchmark]
    public string NaiveConcatenation()
    {
        // Manual string concatenation (Painful and error-prone)
        var sb = new StringBuilder();
        
        // Payload Format Indicator
        sb.Append("000201");
        
        // Merchant Account Information (Pix Key)
        // Manual length calculation (bad)
        var key = "test@benchmark.com";
        sb.Append("26"); 
        sb.Append((22 + key.Length).ToString("D2")); // 0014br.gov.bcb.pix01 + len
        sb.Append("0014br.gov.bcb.pix");
        sb.Append("01");
        sb.Append(key.Length.ToString("D2"));
        sb.Append(key);
        
        // Merchant Category Code
        sb.Append("52040000");
        
        // Transaction Currency
        sb.Append("5303986");
        
        // Transaction Amount
        var amount = "123.45";
        sb.Append("54");
        sb.Append(amount.Length.ToString("D2"));
        sb.Append(amount);
        
        // Country Code
        sb.Append("5802BR");
        
        // Merchant Name
        var name = "Loja Benchmark";
        sb.Append("59");
        sb.Append(name.Length.ToString("D2"));
        sb.Append(name);
        
        // Merchant City
        var city = "Sao Paulo";
        sb.Append("60");
        sb.Append(city.Length.ToString("D2"));
        sb.Append(city);
        
        // TxId
        var txId = "BENCH01";
        sb.Append("62");
        sb.Append((4 + txId.Length).ToString("D2"));
        sb.Append("05");
        sb.Append(txId.Length.ToString("D2"));
        sb.Append(txId);
        
        // CRC Placeholder
        sb.Append("6304");

        // Calculate CRC
        var payloadForCrc = sb.ToString();
        var crc = NaiveCrc16(payloadForCrc);
        
        return payloadForCrc + crc;
    }

    // Typical implementation found on StackOverflow
    private string NaiveCrc16(string payload)
    {
        var bytes = Encoding.UTF8.GetBytes(payload);
        ushort crc = 0xFFFF;
        ushort poly = 0x1021;

        foreach (byte b in bytes)
        {
            for (int i = 0; i < 8; i++)
            {
                bool bit = ((b >> (7 - i) & 1) == 1);
                bool c15 = ((crc >> 15 & 1) == 1);
                crc <<= 1;
                if (c15 ^ bit) crc ^= poly;
            }
        }

        return crc.ToString("X4");
    }
}
