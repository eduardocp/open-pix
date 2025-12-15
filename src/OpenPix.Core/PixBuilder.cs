using System.Text;
using OpenPix.Core.Domain;
using OpenPix.Core.Infra;

namespace OpenPix.Core;

public class PixBuilder
{
    // Estado interno usando Value Objects (Nullables até o Build)
    private string? _key;
    private Merchant? _merchant;
    private TransactionId _txId = TransactionId.Default;
    private decimal? _amount;

    public static PixBuilder Create() => new();

    public PixBuilder WithKey(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        _key = key.Trim();
        return this;
    }

    public PixBuilder WithMerchant(string name, string city)
    {
        // A validação acontece dentro da classe Merchant
        _merchant = new Merchant(name, city);
        return this;
    }

    public PixBuilder WithAmount(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive.");

        _amount = amount;
        return this;
    }

    public PixBuilder WithTransactionId(string txId)
    {
        // A validação acontece dentro da classe TransactionId
        _txId = new TransactionId(txId);
        return this;
    }

    public string Build()
    {
        // Fail Fast: Valida estado antes de começar
        ValidateState();

        var sb = new StringBuilder();

        // Cabeçalhos Fixos
        sb.Append(EmvCodec.Format("00", "01"));

        // Tag 26: Info da Conta
        var accountInfo = new StringBuilder()
            .Append(EmvCodec.Format("00", "br.gov.bcb.pix"))
            .Append(EmvCodec.Format("01", _key!))
            .ToString();

        sb.Append(EmvCodec.Format("26", accountInfo));

        // Metadata Fixa
        sb.Append(EmvCodec.Format("52", "0000")); // Merchant Category
        sb.Append(EmvCodec.Format("53", "986"));  // Currency BRL

        // Valor (Opcional)
        if (_amount.HasValue)
        {
            sb.Append(EmvCodec.Format("54", EmvCodec.FormatAmount(_amount.Value)));
        }

        // Dados do Recebedor
        sb.Append(EmvCodec.Format("58", "BR"));
        sb.Append(EmvCodec.Format("59", _merchant!.Name));
        sb.Append(EmvCodec.Format("60", _merchant.City));

        // TxID (Tag 62, Subtag 05)
        var txIdContent = EmvCodec.Format("05", _txId.Value);
        sb.Append(EmvCodec.Format("62", txIdContent));

        // Finaliza com CRC
        return EmvCodec.AssemblePayload(sb);
    }

    private void ValidateState()
    {
        if (_key is null)
            throw new InvalidOperationException("Pix Key is required. Use .WithKey().");

        if (_merchant is null)
            throw new InvalidOperationException("Merchant info is required. Use .WithMerchant().");
    }
}