using Cinema.Application.DTOs.Combos;
using Cinema.Application.DTOs.Common;
using Cinema.Application.Interfaces;
using Cinema.Domain.Entities;
using Cinema.Shared.Exceptions;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Services.Combos;

public class ComboService(IAppDbContext db) : IComboService
{
    public async Task<List<ComboDto>> GetActiveCombosAsync(CancellationToken cancellationToken) =>
        await db
            .Combos.Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ProjectToType<ComboDto>()
            .ToListAsync(cancellationToken);

    public async Task<OffsetPageResult<AdminComboDto>> GetCombosAdminAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = db.Combos.OrderBy(c => c.Name);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<AdminComboDto>()
            .ToListAsync(cancellationToken);
        return OffsetPageResult<AdminComboDto>.Create(items, page, pageSize, total);
    }

    public async Task<AdminComboDto> CreateComboAsync(
        UpsertComboRequest request,
        CancellationToken cancellationToken
    )
    {
        var combo = new Combo
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            ImageUrl = request.ImageUrl,
            IsActive = true,
        };
        db.Combos.Add(combo);
        await db.SaveChangesAsync(cancellationToken);
        return combo.Adapt<AdminComboDto>();
    }

    public async Task<AdminComboDto> UpdateComboAsync(
        int comboId,
        UpsertComboRequest request,
        CancellationToken cancellationToken
    )
    {
        var combo =
            await db.Combos.FirstOrDefaultAsync(c => c.ComboId == comboId, cancellationToken)
            ?? throw new NotFoundException("Không tìm thấy combo.");

        combo.Name = request.Name;
        combo.Description = request.Description;
        combo.Price = request.Price;
        combo.ImageUrl = request.ImageUrl;
        await db.SaveChangesAsync(cancellationToken);
        return combo.Adapt<AdminComboDto>();
    }

    public async Task<ToggleComboResponse> ToggleComboAsync(
        int comboId,
        CancellationToken cancellationToken
    )
    {
        var combo =
            await db.Combos.FirstOrDefaultAsync(c => c.ComboId == comboId, cancellationToken)
            ?? throw new NotFoundException("Không tìm thấy combo.");

        combo.IsActive = !combo.IsActive;
        await db.SaveChangesAsync(cancellationToken);
        return new ToggleComboResponse(comboId, combo.IsActive);
    }
}
