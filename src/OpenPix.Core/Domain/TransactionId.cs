using System.Text.RegularExpressions;

namespace OpenPix.Core.Domain;

public partial record TransactionId
{
    public string Value { get; }

    private static readonly Regex Validator = ValidatorRegexCompiled();

    public static TransactionId Default => new("***");

    public TransactionId(string? value)
    {
        // FIX: Explicitly accept null, empty OR the default "***"
        if (string.IsNullOrEmpty(value) || value == "***")
        {
            Value = "***";
            return;
        }

        if (value.Length > 25)
            throw new ArgumentException("Transaction ID cannot exceed 25 characters.", nameof(value));

        if (!Validator.IsMatch(value))
            throw new ArgumentException("Transaction ID must be alphanumeric.", nameof(value));

        Value = value;
    }

    [GeneratedRegex("^[a-zA-Z0-9]+$", RegexOptions.Compiled)]
    private static partial Regex ValidatorRegexCompiled();
}