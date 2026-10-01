using System.Security.Claims;
using Cinema.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers;

/// <summary>
/// Base controller providing shared helpers for claim extraction and pagination clamping.
/// Eliminates repeated boilerplate across all controllers.
/// </summary>
public abstract class AppControllerBase : ControllerBase
{
    /// <summary>
    /// Returns the authenticated user's ID. Throws FormatException if claim is missing.
    /// Use only on [Authorize] endpoints where the claim is guaranteed.
    /// </summary>
    protected int CurrentUserId =>
        int.Parse(User.FindFirstValue(AppClaimTypes.UserId)!);

    /// <summary>
    /// Returns the authenticated user's ID, or null if not present / not parseable.
    /// Use on endpoints that are accessible to both authenticated and anonymous users.
    /// </summary>
    protected int? CurrentUserIdOrNull
    {
        get
        {
            var raw = User.FindFirstValue(AppClaimTypes.UserId);
            return int.TryParse(raw, out var id) ? id : null;
        }
    }

    /// <summary>
    /// Returns the authenticated employee's ID from claims.
    /// Must be called only from actions guarded by StaffOnly/AdminOnly policy.
    /// Returns null if the claim is absent (caller should guard appropriately).
    /// </summary>
    protected int? CurrentEmployeeIdOrNull
    {
        get
        {
            var raw = User.FindFirstValue(AppClaimTypes.EmployeeId);
            return int.TryParse(raw, out var id) ? id : null;
        }
    }

    /// <summary>
    /// Clamps a nullable cursor-pagination limit to [1, 50].
    /// </summary>
    protected static int ClampLimit(int? limit) =>
        Math.Clamp(limit ?? CursorPagination.DefaultLimit, 1, 50);
}
