using System.Text;
using OpenPix.Core.Domain;
using OpenPix.Core.Infra;

namespace OpenPix.Core;

public class PixBuilder
{
    // Internal state using Value Objects (Nullables until Build)
    private string? _key;
    private string? _url;
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

    public PixBuilder WithMerchant(string name, string city, string? zipCode = null)
    {
        // Validation happens inside the Merchant class
        _merchant = new Merchant(name, city, zipCode);
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
        // Validation happens inside the TransactionId class
        _txId = new TransactionId(txId);
        return this;
    }

    public PixBuilder WithDynamicUrl(string url)
    {
        // The URL must start with https:// and typically the bank validates the domain
        if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));
        if (!url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("A URL do Pix Dinâmico deve ser HTTPS.", nameof(url));

        _url = url;
        return this;
    }

    public string Build()
    {
        // Fail Fast: Validate state before starting
        ValidateState();

        var sb = new StringBuilder();

        // Fixed Headers
        sb.Append(EmvCodec.Format("00", "01"));

        // Tag 26: Merchant Account Information
        var accountInfoSb = new StringBuilder();
        accountInfoSb.Append(EmvCodec.Format("00", "br.gov.bcb.pix"));

        if (!string.IsNullOrEmpty(_url))
        {
            // Dynamic Pix uses ID 25 for the URL
            // Important: The URL cannot have 'https://' in the final payload, only the domain/path
            // But the protocol removal rule varies by PSP. 
            // The BACEN standard says to use the full string, but many remove the protocol.
            // Let's assume the passed URL is the correct one provided by the bank.
            accountInfoSb.Append(EmvCodec.Format("25", _url));
        }
        else
        {
            // Static Pix uses ID 01 for the Key
            accountInfoSb.Append(EmvCodec.Format("01", _key!));
        }

        sb.Append(EmvCodec.Format("26", accountInfoSb.ToString()));

        // Fixed Metadata
        sb.Append(EmvCodec.Format("52", "0000")); // Merchant Category
        sb.Append(EmvCodec.Format("53", "986"));  // Currency BRL

        // Value (Optional)
        if (_amount.HasValue)
        {
            sb.Append(EmvCodec.Format("54", EmvCodec.FormatAmount(_amount.Value)));
        }

        // Receiver Data
        sb.Append(EmvCodec.Format("58", "BR"));
        sb.Append(EmvCodec.Format("59", _merchant!.Name));
        sb.Append(EmvCodec.Format("60", _merchant.City));

        if (!string.IsNullOrEmpty(_merchant.ZipCode))
        {
            sb.Append(EmvCodec.Format("61", _merchant.ZipCode));
        }

        // TxID (Tag 62, Subtag 05)
        var txIdContent = EmvCodec.Format("05", _txId.Value);
        sb.Append(EmvCodec.Format("62", txIdContent));

        // Finishes with CRC
        return EmvCodec.AssemblePayload(sb);
    }

    private void ValidateState()
    {
        // Rule 1: Must have at least one
        if (_key is null && _url is null)
            throw new InvalidOperationException("Configure uma Chave (.WithKey) ou URL (.WithDynamicUrl) antes de gerar.");

        // Rule 2: Cannot have both at the same time
        if (_key is not null && _url is not null)
            throw new InvalidOperationException("Não é possível configurar Chave (.WithKey) e URL (.WithDynamicUrl) ao mesmo tempo.");

        if (_merchant is null)
            throw new InvalidOperationException("Merchant info is required.");
    }
}