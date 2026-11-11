using Cinema.Application.DTOs.Payments;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Mapster;

namespace Cinema.Application.MappingConfig;

public class PaymentMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<Payment, AdminPaymentListItemDto>()
            .Map(dest => dest.BookingCode, src => src.Booking.BookingCode)
            .Map(dest => dest.PaymentMethod, src => src.PaymentMethod.ToDtoMethod())
            .Map(dest => dest.Status, src => src.Status.ToDtoStatus());

        config
            .NewConfig<Payment, AdminPaymentDetailDto>()
            .Map(dest => dest.PaymentMethod, src => src.PaymentMethod.ToDtoMethod())
            .Map(dest => dest.Status, src => src.Status.ToDtoStatus())
            .Map(
                dest => dest.Booking,
                src => new AdminPaymentBookingBriefDto(
                    src.Booking.BookingId,
                    src.Booking.BookingCode,
                    src.Booking.Status.ToDtoStatus(),
                    src.Booking.FinalAmount
                )
            );
    }
}
