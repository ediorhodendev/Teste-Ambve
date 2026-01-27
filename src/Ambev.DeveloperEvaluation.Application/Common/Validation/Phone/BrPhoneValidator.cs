using System.Text.RegularExpressions;

namespace Ambev.DeveloperEvaluation.Application.Common.Validation.Phone;

public static class BrPhoneValidator
{
    public static bool IsValid(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return false;

        var digits = Regex.Replace(phone, @"\D", "");
        return digits.Length is >= 10 and <= 13;
    }

    public static string Normalize(string phone)
        => Regex.Replace(phone ?? "", @"\D", "");
}
