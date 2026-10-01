namespace Cinema.Application.DTOs.Combos;

public record ComboDto(
    int ComboId,
    string Name,
    string? Description,
    decimal Price,
    string? ImageUrl
);

public record AdminComboDto(
    int ComboId,
    string Name,
    string? Description,
    decimal Price,
    string? ImageUrl,
    bool IsActive
);

public record UpsertComboRequest(string Name, string? Description, decimal Price, string? ImageUrl);

public record ToggleComboResponse(int ComboId, bool IsActive);
