using System.Text;

namespace Cinema.Application.Utils;
public static class NameNormalizer
{
    public static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var composed = ToComposedForm(value);
        var builder = new StringBuilder(composed.Length);
        var pendingSpace = false;

        foreach (var ch in composed)
        {
            if (char.IsWhiteSpace(ch))
            {
                pendingSpace = builder.Length > 0;
                continue;
            }

            if (pendingSpace)
            {
                builder.Append(' ');
                pendingSpace = false;
            }

            builder.Append(ch);
        }

        return builder.ToString();
    }

    public static bool AreSame(string? left, string? right) =>
        string.Equals(Normalize(left), Normalize(right), StringComparison.OrdinalIgnoreCase);

    public static bool ContainsDisallowedCharacters(string? value) =>
        value is not null && value.Any(ch => char.IsControl(ch) && !char.IsWhiteSpace(ch));

    private static string ToComposedForm(string value)
    {
        try
        {
            return value.Normalize(NormalizationForm.FormC);
        }
        catch (ArgumentException)
        {
            return value;
        }
    }
}
