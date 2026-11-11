namespace Cinema.Application.Options;

public static class ProductionOptionsValidator
{
    private const int MinJwtSecretLength = 32;

    public static IReadOnlyList<string> Validate(
        AppUrlOptions app,
        JwtOptions jwt,
        IReadOnlyCollection<string> corsOrigins,
        VnPayOptions vnPay,
        MomoOptions momo,
        StripeOptions stripe,
        BrandOptions brand
    )
    {
        var errors = new List<string>();

        if (jwt.Secret.Length < MinJwtSecretLength)
            errors.Add($"Jwt:Secret chưa cấu hình hoặc ngắn hơn {MinJwtSecretLength} ký tự.");

        RequirePublicUrl(errors, "App:ApiBaseUrl", app.ApiBaseUrl);
        RequirePublicUrl(errors, "App:FrontendBaseUrl", app.FrontendBaseUrl);

        RequireValue(errors, "Brand:TimeZoneId", brand.TimeZoneId);

        if (corsOrigins.Count == 0)
            errors.Add("Cors:AllowedOrigins chưa cấu hình.");

        if (IsAnySet(vnPay.TmnCode, vnPay.HashSecret))
        {
            RequireValue(errors, "VnPay:TmnCode", vnPay.TmnCode);
            RequireValue(errors, "VnPay:HashSecret", vnPay.HashSecret);
            RequirePublicUrl(errors, "VnPay:ReturnUrl", vnPay.ReturnUrl);
            RequirePublicUrl(errors, "VnPay:IpnUrl", vnPay.IpnUrl);
        }

        if (IsAnySet(momo.PartnerCode, momo.AccessKey, momo.SecretKey))
        {
            RequireValue(errors, "Momo:PartnerCode", momo.PartnerCode);
            RequireValue(errors, "Momo:AccessKey", momo.AccessKey);
            RequireValue(errors, "Momo:SecretKey", momo.SecretKey);
            RequirePublicUrl(errors, "Momo:ReturnUrl", momo.ReturnUrl);
            RequirePublicUrl(errors, "Momo:IpnUrl", momo.IpnUrl);
        }

        if (IsAnySet(stripe.SecretKey))
            RequirePublicUrl(errors, "Stripe:ReturnUrl", stripe.ReturnUrl);

        return errors;
    }

    private static bool IsAnySet(params string[] values) =>
        values.Any(v => !string.IsNullOrWhiteSpace(v));

    private static void RequireValue(List<string> errors, string key, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            errors.Add($"{key} chưa cấu hình.");
    }

    private static void RequirePublicUrl(List<string> errors, string key, string value)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) || uri.IsLoopback)
            errors.Add($"{key} chưa cấu hình hoặc đang trỏ về localhost.");
    }
}
