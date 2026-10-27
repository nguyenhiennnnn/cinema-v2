using Cinema.Application.DTOs.Combos;
using Cinema.Application.DTOs.Common;

namespace Cinema.Application.Interfaces;

public interface IComboService
{
    Task<List<ComboDto>> GetActiveCombosAsync(CancellationToken cancellationToken);

    Task<List<ComboSuggestionDto>> GetComboSuggestionsAsync(
        int showtimeId,
        int seatCount,
        CancellationToken cancellationToken
    );

    Task<OffsetPageResult<AdminComboDto>> GetCombosAdminAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );
    Task<AdminComboDto> CreateComboAsync(
        UpsertComboRequest request,
        CancellationToken cancellationToken
    );
    Task<AdminComboDto> UpdateComboAsync(
        int comboId,
        UpsertComboRequest request,
        CancellationToken cancellationToken
    );

    Task<ToggleComboResponse> ToggleComboAsync(int comboId, CancellationToken cancellationToken);
}
