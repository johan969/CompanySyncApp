using System.Globalization;
using System.Text;

public static class NameNormalizer
{
    public static string Normalize(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var normalized = value
            .Trim()
            .Normalize(NormalizationForm.FormKC);

        normalized = string.Join(
            ' ',
            normalized.Split(
                (char[]?)null,
                StringSplitOptions.RemoveEmptyEntries));

        return normalized.ToUpperInvariant();
    }
}