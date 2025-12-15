using System.Text.RegularExpressions;

namespace OpenPix.Core.Domain;

public enum PixKeyType
{
    Cpf,
    Cnpj,
    Email,
    Phone,
    Random,
    Unknown
}

public static class PixKeyValidator
{
    // Regex Patterns
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    private static readonly Regex PhoneRegex = new(@"^\+55\d{10,11}$", RegexOptions.Compiled);
    private static readonly Regex RandomKeyRegex = new(@"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex CpfRegex = new(@"^\d{11}$", RegexOptions.Compiled);
    private static readonly Regex CnpjRegex = new(@"^\d{14}$", RegexOptions.Compiled);

    public static PixKeyType DetermineType(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return PixKeyType.Unknown;

        if (EmailRegex.IsMatch(key)) return PixKeyType.Email;
        if (RandomKeyRegex.IsMatch(key)) return PixKeyType.Random;
        if (PhoneRegex.IsMatch(key)) return PixKeyType.Phone;
        if (CpfRegex.IsMatch(key)) return PixKeyType.Cpf;
        if (CnpjRegex.IsMatch(key)) return PixKeyType.Cnpj;

        return PixKeyType.Unknown;
    }

    public static bool IsValid(string key)
    {
        return DetermineType(key) != PixKeyType.Unknown;
    }
}
