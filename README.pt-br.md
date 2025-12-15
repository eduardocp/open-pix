# OpenPix 🚀

**Uma biblioteca .NET de alto desempenho e código limpo para lidar com pagamentos PIX (EMV BR Code).**

[![Build Status](https://img.shields.io/github/actions/workflow/status/eduardocp/open-pix/dotnet.yml?branch=main)](https://github.com/eduardocp/open-pix/actions)
[![NuGet](https://img.shields.io/nuget/v/OpenPix.Core.svg)](https://www.nuget.org/packages/OpenPix.Core)
[![License](https://img.shields.io/github/license/eduardocp/open-pix)](LICENSE)
[![codecov](https://codecov.io/github/eduardocp/open-pix/graph/badge.svg?token=GDCOG0D4UR)](https://codecov.io/github/eduardocp/open-pix)

> 🇺🇸 [Read in English](README.md)

## 💡 Por que OpenPix?

A maioria das implementações de PIX em .NET depende de concatenação de strings e carece de validação adequada. O **OpenPix** foi construído com **Domain-Driven Design (DDD)** e **Desempenho** em mente.

- **⚡ Alto Desempenho:** Utiliza `ReadOnlySpan<char>` para o parsing, evitando alocações desnecessárias de memória (strings).
- **🛡️ Validação Blindada:** Valida CRC-16, tamanhos de campos EMV e **dígitos verificadores de CPF/CNPJ (Mod11)**, garantindo que chaves inválidas não sejam geradas.
- **✨ Código Limpo:** Expõe uma API fluente e Objetos de Domínio ricos (`Merchant`, `TransactionId`) em vez de strings cruas.
- **🔗 Dinâmico & Estático:** Suporta tanto PIX Estático (Chave Pix) quanto PIX Dinâmico (URL do PSP/Banco).
- **📦 Modular:** A biblioteca Core (`OpenPix.Core`) tem **zero dependências**.

---

## 🚀 Instalação

### 1. O Core (Parser & Gerador)

Leve, lógica pura, zero dependências.

```bash
dotnet add package OpenPix.Core
```

### 2. Integração ASP.NET Core (Injeção de Dependência)

Configuração global e injeção para Web APIs.

```bash
dotnet add package OpenPix.AspNetCore
```

### 3. Extensão Visual (Opcional)

Se você precisa renderizar a imagem do QR Code (PNG/SVG).

```bash
dotnet add package OpenPix.QRCode
```

---

## ⚡ Benchmarks

O OpenPix é otimizado para cenários de alto tráfego.

### 1. Leitura (Parsing)

Comparando o `PixParser` contra manipulação de strings tradicional:

| Método               | Média        | Alocado   | Razão     |
| :------------------- | :----------- | :-------- | :-------- |
| **OpenPix**          | **2.664 μs** | **272 B** | **1.00x** |
| Implementação Comum  | 10.263 μs    | 15,824 B  | 3.85x     |

### 2. Geração (Builder)

Comparando `PixBuilder` (API Fluente + Validação Completa) contra concatenação manual de strings:

| Método      | Média       | Alocado     | Benefícios                                 |
| :---------- | :---------- | :---------- | :----------------------------------------- |
| **OpenPix** | **1.48 μs** | **1.83 KB** | **Menos Memória**, Validação, Código Limpo |
| Manual      | 1.50 μs     | 3.02 KB     | Propenso a erros, Difícil manutenção       |

_> **Resultado:** OpenPix permite escrever **código mais limpo e seguro** usando **40% menos memória** que a concatenação manual._

---

## 📖 Como Usar

### 1. Gerando um PIX Estático (Chave Pix)

Ideal para pequenos negócios ou transferências P2P.

```csharp
using OpenPix;

var payload = PixBuilder.Create()
    .WithKey("usuario@exemplo.com")
    .WithMerchant("Nome Da Loja", "Sao Paulo", "12345-000") // CEP (Opcional)
    .WithAmount(12.50m)
    .WithTransactionId("PEDIDO12345")
    .Build();
```

> **Nota:** O OpenPix valida automaticamente os dígitos verificadores de CPF/CNPJ. Se você passar uma chave inválida (erro de digitação), ele lançará uma exceção imediatamente para evitar gerar um QR Code inútil.

### 2. Gerando um PIX Dinâmico (URL do PSP)

Ideal para integrações de e-commerce onde o Banco/PSP fornece uma URL única (Location).

```csharp
var payload = PixBuilder.Create()
    .WithDynamicUrl("https://pix.exemplo.com/qr/v2/9d36b84f-70b3-40a1")
    .WithMerchant("Nome Da Loja", "Sao Paulo")
    .WithAmount(50.00m) // Opcional para Dinâmico, mas recomendado para exibição
    .Build();
```

### 3. Fazendo Parsing e Validando (Leitura)

Leia uma string bruta, valide sua assinatura CRC-16 e hidrate um Objeto de Domínio rico.

```csharp
using OpenPix;

var rawString = "00020126..."; // Entrada de um usuário ou scanner

try
{
    var pixData = PixParser.Parse(rawString);

    if (!string.IsNullOrEmpty(pixData.Url))
    {
        Console.WriteLine($"URL Dinâmica: {pixData.Url}");
    }
    else
    {
        Console.WriteLine($"Chave Pix: {pixData.PixKey}");
    }

    Console.WriteLine($"Recebedor: {pixData.Merchant?.Name}");
    Console.WriteLine($"Valor:     {pixData.Amount:C}");
    Console.WriteLine($"TxID:      {pixData.TxId.Value}");
}
catch (ArgumentException ex)
{
    Console.WriteLine("Código PIX inválido ou erro de checksum.");
}
```

### 4. Renderizando o QR Code

Se você instalou o `OpenPix.QRCode`, pode converter a string diretamente para uma imagem.

```csharp
using OpenPix;
using OpenPix.QRCode; // Importar métodos de extensão

var payload = PixBuilder.Create()...Build();

// Gera uma string Base64 pronta para <img src="...">
// Automaticamente define fundo branco e módulos pretos para compatibilidade com apps de banco.
string base64Png = payload.ToPngBase64(pixelsPerModule: 10);

// Gera uma string SVG para gráficos vetoriais escaláveis
string svgContent = payload.ToSvg();

// Gera uma arte ASCII para aplicações de console
Console.WriteLine(payload.ToAsciiArt());
```

### 5. Integração com ASP.NET Core

Exponha facilmente um endpoint que gera QR Codes Pix dinamicamente usando nossa extensão para Minimal APIs.

**Program.cs:**

```csharp
using OpenPix.AspNetCore; // Importar namespace

var builder = WebApplication.CreateBuilder(args);

// 1. Adicionar serviços OpenPix
// Configure a Chave Pix no appsettings.json ou via options explicitas
builder.Services.AddOpenPix(options =>
{
    options.PixKey = "user@example.com";
    options.MerchantName = "Minha Loja";
    options.City = "Sao Paulo";
});

var app = builder.Build();

// 2. Mapear o endpoint gerador (retorna imagem PNG)
app.MapPixQrCode("/api/pix/qrcode"); 
// Exemplo de URL: /api/pix/qrcode?amount=10.50&txid=PEDIDO123

app.Run();
```

---

## 🖥️ Ferramenta CLI

Você pode usar o OpenPix diretamente do seu terminal para gerar e ler códigos PIX.

### Instalação

```bash
# Rodar a partir do código fonte (dev)
dotnet run --project src/OpenPix.Cli -- --help

# Ou instalar como ferramenta global (uma vez empacotado)
dotnet tool install -g OpenPix.Cli
```

### Como Usar

**Gerar um Pix:**
```bash
openpix gen --name "Minha Loja" --city "Sao Paulo" --zip "12345-000" --key "usuario@exemplo.com" --amount 10.50
```

**Ler (Decodificar) um Pix:**
```bash
openpix decode "00020126..."
```

---

## 🏗️ Arquitetura

Este projeto segue princípios de **Clean Architecture**:

- **OpenPix.Core:**
  - `Domain`: Contém Value Objects (`Merchant`, `TransactionId`) que aplicam regras de negócio na instanciação.
  - `Infra`: Contém algoritmos de baixo nível como `Crc16` e `EmvCodec`.
  - Sem dependências externas.
- **OpenPix.QRCode:**
  - Depende de `QRCoder` para lidar com a geração da matriz gráfica.
  - Estende a funcionalidade do Core.

* **OpenPix.AspNetCore:** Extensões de `IServiceCollection` e `IPixClient` para Web APIs.

---

## 🤝 Contribuição

Contribuições são bem-vindas! Por favor, verifique a aba [Issues](https://github.com/eduardocp/open-pix/issues).

1.  Faça um Fork do projeto.
2.  Crie sua Feature Branch (`git checkout -b feature/RecursoIncrivel`).
3.  Faça o Commit de suas mudanças (`git commit -m 'Adiciona algum RecursoIncrivel'`).
4.  Faça o Push para a Branch (`git push origin feature/RecursoIncrivel`).
5.  Abra um Pull Request.

## 📄 Licença

Distribuído sob a Licença MIT. Veja `LICENSE` para mais informações.
