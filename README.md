# OpenPix 🚀

Uma biblioteca .NET moderna, leve e focada em performance para manipulação de PIX (EMV BR Code).

[![NuGet](https://img.shields.io/nuget/v/OpenPix.svg)](https://www.nuget.org/packages/OpenPix)
[![Build Status](https://github.com/eduardocp/open-pix/actions/workflows/dotnet.yml/badge.svg)](https://github.com/eduardocp/open-pix/actions)

## Por que usar?

- **Zero Alocação (Parsing):** Usa `ReadOnlySpan<char>` para ler PIX sem alocar strings desnecessárias.
- **Clean Code:** API fluente e domínio rico (`ValueObjects` que impedem estados inválidos).
- **Segurança:** Validação rigorosa de CRC-16 e regras do BACEN.
- **Zero Dependências:** Apenas .NET 8 puro.

## Como usar

### Gerando um PIX (Static)

```csharp
using OpenPix;

var payload = PixBuilder.Create()
    .WithKey("seu-email@chave.com")
    .WithMerchant("Minha Loja", "São Paulo")
    .WithAmount(100.00m)
    .WithTransactionId("PEDIDO123") // Validação automática de padrão EMV
    .Build();

// Gera: 00020126... (Pronto para QRCode)
```
