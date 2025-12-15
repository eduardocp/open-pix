# OpenPix 🚀

**A high-performance, clean-code .NET library for handling Brazilian PIX (EMV BR Code) payments.**

[![Build Status](https://img.shields.io/github/actions/workflow/status/eduardocp/open-pix/dotnet.yml?branch=main)](https://github.com/eduardocp/open-pix/actions)
[![NuGet](https://img.shields.io/nuget/v/OpenPix.Core.svg)](https://www.nuget.org/packages/OpenPix.Core)
[![License](https://img.shields.io/github/license/eduardocp/open-pix)](LICENSE)
[![codecov](https://codecov.io/github/eduardocp/open-pix/graph/badge.svg?token=GDCOG0D4UR)](https://codecov.io/github/eduardocp/open-pix)


> 🇧🇷 [Leia em Português](README.pt-br.md)

## 💡 Why OpenPix?

Most PIX implementations in .NET rely on string concatenation and lack proper validation. **OpenPix** was built with **Domain-Driven Design (DDD)** and **Performance** in mind.

- **⚡ High Performance:** Uses `ReadOnlySpan<char>` for parsing, avoiding unnecessary string allocations.
- **🛡️ Bulletproof Validation:** Validates CRC-16 checksums, EMV field lengths, and character sets automatically.
- **✨ Clean Code:** Exposes a fluent API and rich Domain Objects (`Merchant`, `TransactionId`) instead of raw strings.
- **🔗 Dynamic & Static:** Supports both Static PIX (Pix Key) and Dynamic PIX (PSP/Bank URL).
- **📦 Modular:** The Core library (`OpenPix.Core`) has **zero dependencies**.

---

## 🚀 Installation

### 1. The Core (Parser & Generator)

Lightweight, pure logic, zero dependencies.

```bash
dotnet add package OpenPix.Core
```

### 2. ASP.NET Core Integration (Dependency Injection)

Global configuration and injection for Web APIs.

```bash
dotnet add package OpenPix.AspNetCore
```

### 3. Visual Extension (Optional)

If you need to render the QR Code image (PNG/SVG).

```bash
dotnet add package OpenPix.QRCode
```

---

## ⚡ Benchmarks

OpenPix is optimized for high-throughput scenarios. Comparing `PixParser` against a traditional naive implementation:

| Method               | Mean Speed   | Ratio     |
| :------------------- | :----------- | :-------- |
| **OpenPix**          | **1.231 us** | **1.00x** |
| Naive Implementation | 6.009 us     | 4.88x     |

_> **Result:** OpenPix is approximately **5x faster** than traditional approaches._

---

## 📖 Usage

### 1. Generating a Static PIX (Pix Key)

Ideal for small businesses or P2P transfers.

```csharp
using OpenPix;

var payload = PixBuilder.Create()
    .WithKey("user@example.com")
    .WithMerchant("My Store Name", "Sao Paulo", "12345-000") // ZipCode (Optional)
    .WithAmount(12.50m)
    .WithTransactionId("ORDER12345")
    .Build();
```

### 2. Generating a Dynamic PIX (PSP URL)

Ideal for e-commerce integrations where the Bank/PSP provides a unique URL (Location).

```csharp
var payload = PixBuilder.Create()
    .WithDynamicUrl("https://pix.example.com/qr/v2/9d36b84f-70b3-40a1")
    .WithMerchant("My Store Name", "Sao Paulo")
    .WithAmount(50.00m) // Optional for Dynamic, but recommended for display
    .Build();
```

### 3. Parsing and Validating (Reader)

Read a raw string, validate its CRC-16 signature, and hydrate a rich Domain Object.

```csharp
using OpenPix;

var rawString = "00020126..."; // Input from a user or scanner

try
{
    var pixData = PixParser.Parse(rawString);

    if (!string.IsNullOrEmpty(pixData.Url))
    {
        Console.WriteLine($"Dynamic URL: {pixData.Url}");
    }
    else
    {
        Console.WriteLine($"Pix Key: {pixData.PixKey}");
    }

    Console.WriteLine($"Merchant: {pixData.Merchant?.Name}");
    Console.WriteLine($"Amount:   {pixData.Amount:C}");
    Console.WriteLine($"TxID:     {pixData.TxId.Value}");
}
catch (ArgumentException ex)
{
    Console.WriteLine("Invalid PIX Code or Checksum mismatch.");
}
```

### 4. Rendering the QR Code

If you installed `OpenPix.QRCode`, you can convert the string directly to an image.

```csharp
using OpenPix;
using OpenPix.QRCode; // Import extension methods

var payload = PixBuilder.Create()...Build();

// Generates a Base64 string ready for <img src="...">
// Automatically sets white background/black modules for banking app compatibility.
string base64Png = payload.ToPngBase64(pixelsPerModule: 10);

// Generates an SVG string for scalable vector graphics
string svgContent = payload.ToSvg();

// Generates an ASCII Art string for console applications
Console.WriteLine(payload.ToAsciiArt());
```

---

## 🖥️ CLI Tool

You can use OpenPix directly from your terminal to generate and decode PIX strings.

### Installation

```bash
# Run from source (dev)
dotnet run --project src/OpenPix.Cli -- --help

# Or install as a global tool (once packed)
dotnet tool install -g OpenPix.Cli
```

### Usage

**Generate a Pix:**
```bash
openpix gen --name "My Store" --city "Sao Paulo" --zip "12345-000" --key "user@example.com" --amount 10.50
```

**Decode a Pix:**
```bash
openpix decode "00020126..."
```

---

## 🏗️ Architecture

This project follows **Clean Architecture** principles:

- **OpenPix.Core:**
  - `Domain`: Contains Value Objects (`Merchant`, `TransactionId`) that enforce business rules upon instantiation.
  - `Infra`: Contains low-level algorithms like `Crc16` and `EmvCodec`.
  - No external dependencies.
- **OpenPix.QRCode:**
  - Depends on `QRCoder` to handle the graphical matrix generation.
  - Extends the Core functionality.

* **OpenPix.AspNetCore:** `IServiceCollection` extensions and `IPixClient` for Web APIs.

---

## 🤝 Contribution

Contributions are welcome! Please check the [Issues](https://github.com/eduardocp/open-pix/issues) tab.

1.  Fork the project.
2.  Create your Feature Branch (`git checkout -b feature/AmazingFeature`).
3.  Commit your changes (`git commit -m 'Add some AmazingFeature'`).
4.  Push to the Branch (`git push origin feature/AmazingFeature`).
5.  Open a Pull Request.

## 📄 License

Distributed under the MIT License. See `LICENSE` for more information.
