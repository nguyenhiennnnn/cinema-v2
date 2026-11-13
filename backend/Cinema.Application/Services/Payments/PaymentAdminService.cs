using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Payments;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Services.Payments;

public class PaymentAdminService(IAppDbContext db) : IPaymentAdminService
{
    public async Task<OffsetPageResult<AdminPaymentListItemDto>> GetPaymentsAsync(
        AdminPaymentListFilter filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = db.Payments.Include(p => p.Booking).AsQueryable().ApplyAdminFilter(filter);

        query = query.OrderByDescending(p => p.CreatedAt);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<AdminPaymentListItemDto>()
            .ToListAsync(cancellationToken);

        return OffsetPageResult<AdminPaymentListItemDto>.Create(items, page, pageSize, total);
    }

    public async Task<AdminPaymentDetailDto> GetPaymentDetailAsync(
        int paymentId,
        CancellationToken cancellationToken
    )
    {
        var payment =
            await db
                .Payments.Include(p => p.Booking)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_Payment);

        return payment.Adapt<AdminPaymentDetailDto>();
    }
}
