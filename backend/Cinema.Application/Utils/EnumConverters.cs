using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;

namespace Cinema.Application.Utils;

public static class EnumConverters
{
    public static string ToDtoStatus(this BookingStatus status) =>
        status switch
        {
            BookingStatus.Pending => "Pending",
            BookingStatus.Confirmed => "Confirmed",
            BookingStatus.Cancelled => "Cancelled",
            _ => "Expired",
        };

    public static string ToDtoChannel(this BookingChannel channel) =>
        channel == BookingChannel.Online ? "Online" : "Counter";

    public static string ToDtoMethod(this PaymentMethod method) =>
        method switch
        {
            PaymentMethod.VNPay => "VNPay",
            PaymentMethod.Momo => "Momo",
            PaymentMethod.Stripe => "Stripe",
            _ => "Cash",
        };

    public static string ToDtoStatus(this PaymentStatus status) =>
        status switch
        {
            PaymentStatus.Pending => "Pending",
            PaymentStatus.Success => "Success",
            PaymentStatus.Failed => "Failed",
            _ => "Refunded",
        };

    public static string ToDtoStatus(this ShowtimeStatus status) =>
        status switch
        {
            ShowtimeStatus.Scheduled => "Scheduled",
            ShowtimeStatus.Cancelled => "Cancelled",
            _ => "Completed",
        };

    public static string ToDtoStatus(this ShowtimeSeatStatus status) =>
        status switch
        {
            ShowtimeSeatStatus.Available => "Available",
            ShowtimeSeatStatus.Held => "Held",
            _ => "Booked",
        };

    public static string ToDtoStatus(this MovieStatus status) =>
        status switch
        {
            MovieStatus.ComingSoon => "ComingSoon",
            MovieStatus.NowShowing => "NowShowing",
            _ => "Ended",
        };

    public static string? ToDtoGender(this Gender? gender) =>
        gender switch
        {
            Gender.Male => "Male",
            Gender.Female => "Female",
            Gender.Other => "Other",
            _ => null,
        };

    public static TEnum ParseEnum<TEnum>(string value, string fieldName = "status")
        where TEnum : struct, Enum
    {
        if (Enum.TryParse<TEnum>(value, true, out var result))
            return result;

        throw new ValidationAppException(
            $"Giá trị '{value}' không hợp lệ cho trường {fieldName}.",
            new Dictionary<string, string[]>
            {
                [fieldName] = [$"Giá trị '{value}' không hợp lệ."],
            }
        );
    }

    public static TEnum? ParseNullableEnum<TEnum>(string? value)
        where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return Enum.TryParse<TEnum>(value, true, out var result) ? result : null;
    }

    public static bool TryParseEnum<TEnum>(string? value, out TEnum result)
        where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = default;
            return false;
        }

        return Enum.TryParse(value, true, out result);
    }
}
