using System.Text.RegularExpressions;
using Cinema.Application.Options;

namespace Cinema.Application.Utils;

public static class PhoneRules
{
    private static Regex _pattern = Build(BrandOptions.DefaultPhoneRegex);

    public static Regex Pattern => _pattern;

    public static void Configure(string phoneRegex)
    {
        if (string.IsNullOrWhiteSpace(phoneRegex))
            throw new InvalidOperationException(
                "Brand:PhoneRegex không được để trống. Kiểm tra appsettings.json."
            );

        _pattern = Build(phoneRegex);
    }

    private static Regex Build(string phoneRegex) => new(phoneRegex, RegexOptions.CultureInvariant);
}
