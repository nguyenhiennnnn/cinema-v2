using System.Text.RegularExpressions;
using Cinema.Application.Options;
using Cinema.Shared.Resources;

namespace Cinema.Application.Utils;

public static class PhoneRules
{
    private static Regex _pattern = Build(BrandOptions.DefaultPhoneRegex);

    public static Regex Pattern => _pattern;

    public static void Configure(string phoneRegex)
    {
        if (string.IsNullOrWhiteSpace(phoneRegex))
            throw new InvalidOperationException(Messages.Config_PhoneRegexRequired);

        _pattern = Build(phoneRegex);
    }

    private static Regex Build(string phoneRegex) => new(phoneRegex, RegexOptions.CultureInvariant);
}
