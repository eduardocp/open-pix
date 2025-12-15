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
        var type = DetermineType(key);

        return type switch
        {
            PixKeyType.Cpf => IsCpfValid(key),
            PixKeyType.Cnpj => IsCnpjValid(key),
            PixKeyType.Unknown => false,
            _ => true // Email, Phone, Random are validated by Regex in DetermineType
        };
    }

    private static bool IsCpfValid(ReadOnlySpan<char> cpf)
    {
        // Already checked for 11 digits by Regex
        
        // Check for known invalid patterns (all digits equal)
        bool allEqual = true;
        for (int i = 1; i < 11; i++)
        {
            if (cpf[i] != cpf[0])
            {
                allEqual = false;
                break;
            }
        }
        if (allEqual) return false;

        var tempCpf = cpf[..9];
        var digit1 = CalculateCpfDigit(tempCpf, 10);
        var digit2 = CalculateCpfDigit(cpf[..10], 11);

        return cpf[9] - '0' == digit1 && cpf[10] - '0' == digit2;
    }

    private static int CalculateCpfDigit(ReadOnlySpan<char> source, int initialWeight)
    {
        int sum = 0;
        int weight = initialWeight;
        foreach (char c in source)
        {
            sum += (c - '0') * weight--;
        }

        int remainder = sum % 11;
        return remainder < 2 ? 0 : 11 - remainder;
    }

    private static bool IsCnpjValid(ReadOnlySpan<char> cnpj)
    {
         // Already checked for 14 digits by Regex
         
         // Validation First Digit
         int[] multiplier1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
         int sum = 0;
         for (int i = 0; i < 12; i++)
            sum += (cnpj[i] - '0') * multiplier1[i];
         
         int remainder = sum % 11;
         int digit1 = remainder < 2 ? 0 : 11 - remainder;
         
         if (cnpj[12] - '0' != digit1) return false;

         // Validation Second Digit
         int[] multiplier2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
         sum = 0;
         for (int i = 0; i < 13; i++)
            sum += (cnpj[i] - '0') * multiplier2[i];
         
         remainder = sum % 11;
         int digit2 = remainder < 2 ? 0 : 11 - remainder;

         return cnpj[13] - '0' == digit2;
    }
}
