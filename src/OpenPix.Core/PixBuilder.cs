using System.Text;
using OpenPix.Core.Domain;
using OpenPix.Core.Infra;

namespace OpenPix.Core;

public class PixBuilder
{
    // Estado interno usando Value Objects (Nullables até o Build)
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

    public PixBuilder WithDynamicUrl(string url)
    {
        // A URL deve começar com https:// e geralmente o banco valida o domínio
        if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));
        if (!url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("A URL do Pix Dinâmico deve ser HTTPS.", nameof(url));

        _url = url;
        return this;
    }

    public string Build()
    {
        // Fail Fast: Valida estado antes de começar
        ValidateState();

        var sb = new StringBuilder();

        // Cabeçalhos Fixos
        sb.Append(EmvCodec.Format("00", "01"));

        // Tag 26: Merchant Account Information
        var accountInfoSb = new StringBuilder();
        accountInfoSb.Append(EmvCodec.Format("00", "br.gov.bcb.pix"));

        // LÓGICA DE DECISÃO: Estático (Chave) vs Dinâmico (URL)
        if (!string.IsNullOrEmpty(_url))
        {
            // Pix Dinâmico usa o ID 25 para a URL
            // Importante: A URL não pode ter 'https://' no payload final, apenas o domínio/caminho
            // Mas a regra de remoção do protocolo varia por PSP. 
            // O padrão do BACEN diz para usar a string completa, mas muitos removem o protocolo.
            // Vamos assumir que a URL passada já é a correta fornecida pelo banco.
            accountInfoSb.Append(EmvCodec.Format("25", _url));
        }
        else
        {
            // Pix Estático usa o ID 01 para a Chave
            accountInfoSb.Append(EmvCodec.Format("01", _key!));
        }

        sb.Append(EmvCodec.Format("26", accountInfoSb.ToString()));

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
        // Validação: Precisa ter OU Chave OU URL
        if (_key is null && _url is null)
            throw new InvalidOperationException("Configure uma Chave (.WithKey) ou URL (.WithDynamicUrl) antes de gerar.");

        if (_merchant is null)
            throw new InvalidOperationException("Merchant info is required.");
    }
}