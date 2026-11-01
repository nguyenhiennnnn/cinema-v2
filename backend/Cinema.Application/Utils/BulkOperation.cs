using Cinema.Application.DTOs.Common;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;

namespace Cinema.Application.Utils;

public static class BulkOperation
{
    public static async Task<BulkActionResponse> RunAsync(
        IEnumerable<int> ids,
        Func<int, Task> action,
        Action<int, Exception>? onUnexpected = null
    )
    {
        var results = new List<BulkActionResultItem>();
        foreach (var id in ids.Distinct())
        {
            try
            {
                await action(id);
                results.Add(new BulkActionResultItem(id, true, null));
            }
            catch (AppException ex)
            {
                results.Add(new BulkActionResultItem(id, false, ex.Message));
            }
            catch (Exception ex)
            {
                onUnexpected?.Invoke(id, ex);
                results.Add(new BulkActionResultItem(id, false, Messages.Bulk_UnknownError));
            }
        }
        return Build(results);
    }

    public static BulkActionResponse Build(List<BulkActionResultItem> results)
    {
        var success = results.Count(r => r.Success);
        return new BulkActionResponse(results, success, results.Count - success);
    }
}
