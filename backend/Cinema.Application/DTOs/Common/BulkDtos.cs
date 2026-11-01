namespace Cinema.Application.DTOs.Common;

public record BulkIdsRequest(List<int> Ids);

public record BulkStatusRequest(List<int> Ids, bool IsActive);

public record BulkActionResultItem(int Id, bool Success, string? Reason);

public record BulkActionResponse(
    List<BulkActionResultItem> Results,
    int TotalSuccess,
    int TotalFailed
);
