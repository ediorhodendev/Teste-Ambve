using System.Text.RegularExpressions;

namespace Ambev.DeveloperEvaluation.Application.Common.Validation.Documents;

public static class BrDocumentValidator
{
    public static bool IsValid(string? document)
    {
        if (string.IsNullOrWhiteSpace(document)) return false;

        var digits = Regex.Replace(document, @"\D", "");
        return digits.Length is 11 or 14;
    }

    public static string Normalize(string document)
        => Regex.Replace(document ?? "", @"\D", "");
}
