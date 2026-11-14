using System.Globalization;
using System.Text;

namespace Cinema.Application.Utils;

public static class ImportHeaderNormalizer
{
    public static string Normalize(string? header)
    {
        if (string.IsNullOrWhiteSpace(header))
            return string.Empty;

        var decomposed = Decompose(header);
        var builder = new StringBuilder(decomposed.Length);

        foreach (var ch in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) == UnicodeCategory.NonSpacingMark)
                continue;

            if (ch is 'đ' or 'Đ')
            {
                builder.Append('d');
                continue;
            }

            if (char.IsLetterOrDigit(ch))
                builder.Append(char.ToLowerInvariant(ch));
        }

        return builder.ToString();
    }

    private static string Decompose(string value)
    {
        try
        {
            return value.Normalize(NormalizationForm.FormD);
        }
        catch (ArgumentException)
        {
            return value;
        }
    }
}
