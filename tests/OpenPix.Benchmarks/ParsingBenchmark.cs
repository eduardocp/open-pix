using BenchmarkDotNet.Attributes;
using OpenPix.Core; // Sua lib
using OpenPix.Core.Domain;

namespace OpenPix.Benchmarks;

[MemoryDiagnoser] // <--- ISSO É O MAIS IMPORTANTE: Mede a alocação de memória (GC)
public class ParsingBenchmark
{
    private string _pixString;

    [GlobalSetup]
    public void Setup()
    {
        // Gera um PIX válido uma única vez para usar nos testes
        _pixString = PixBuilder.Create()
            .WithKey("test@benchmark.com")
            .WithMerchant("Loja Benchmark", "Sao Paulo")
            .WithAmount(123.45m)
            .WithTransactionId("BENCH01")
            .Build();
    }

    // O SEU MÉTODO (Otimizado)
    [Benchmark(Baseline = true)]
    public PixPayload OpenPixParser()
    {
        return PixParser.Parse(_pixString);
    }

    // A CONCORRÊNCIA (Simulação de código comum/lento)
    // Isso simula o que bibliotecas antigas fazem: muito Substring e Alocação
    [Benchmark]
    public void NaiveImplementation()
    {
        var str = _pixString;

        // Simula busca por Substring (Aloca nova string)
        var nome = ExtractNaive(str, "59");
        var cidade = ExtractNaive(str, "60");
        var valor = ExtractNaive(str, "54");

        // Simula criação de objeto sem otimização
        var result = new { Nome = nome, Cidade = cidade, Valor = valor };
    }

    private string ExtractNaive(string fullString, string id)
    {
        // Método ineficiente propositalmente (usa IndexOf e Substring)
        int index = fullString.IndexOf(id);
        if (index == -1) return null;

        int lenIndex = index + 2;
        int length = int.Parse(fullString.Substring(lenIndex, 2));
        return fullString.Substring(lenIndex + 2, length); // <--- ALOCAÇÃO AQUI
    }
}