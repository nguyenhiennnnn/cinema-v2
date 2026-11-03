using System.Security.Claims;
using Cinema.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers;

public abstract class AppControllerBase : ControllerBase
{
    protected int CurrentUserId =>
        int.Parse(User.FindFirstValue(AppClaimTypes.UserId)!);

    protected int? CurrentUserIdOrNull
    {
        get
        {
            var raw = User.FindFirstValue(AppClaimTypes.UserId);
            return int.TryParse(raw, out var id) ? id : null;
        }
    }

    protected int? CurrentEmployeeIdOrNull
    {
        get
        {
            var raw = User.FindFirstValue(AppClaimTypes.EmployeeId);
            return int.TryParse(raw, out var id) ? id : null;
        }
    }

    protected static int ClampLimit(int? limit) =>
        Math.Clamp(limit ?? CursorPagination.DefaultLimit, 1, 50);
}
