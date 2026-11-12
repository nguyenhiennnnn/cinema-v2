using Cinema.Application.DTOs.Employees;
using Cinema.Application.Services.Employees;
using Cinema.Domain.Entities;
using Cinema.UnitTests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class UserAdminServiceUserStatusTests : IDisposable
{
    private readonly PostgresAppDbContextHandle _handle = PostgresAppDbContextHandle.Create();

    private UserAdminService CreateSut()
    {
        _handle.Db.ChangeTracker.Clear();
        return new UserAdminService(_handle.Db);
    }

    private async Task<User> SeedUserWithTokensAsync()
    {
        var user = TestDataSeeder.AddCustomer(_handle.Db);
        _handle.Db.RefreshTokens.AddRange(
            new RefreshToken
            {
                UserId = user.UserId,
                TokenHash = "t1",
                ExpiresAt = DateTime.UtcNow.AddDays(5),
                CreatedAt = DateTime.UtcNow,
            },
            new RefreshToken
            {
                UserId = user.UserId,
                TokenHash = "t2",
                ExpiresAt = DateTime.UtcNow.AddDays(5),
                CreatedAt = DateTime.UtcNow,
            }
        );
        await _handle.Db.SaveChangesAsync();
        return user;
    }

    [Fact]
    public async Task UpdateUserStatusAsync_Deactivate_RevokesAllRefreshTokens()
    {
        var user = await SeedUserWithTokensAsync();
        var bystander = await SeedUserWithTokensAsyncWithEmail("other@test.local");

        await CreateSut()
            .UpdateUserStatusAsync(
                user.UserId,
                new UpdateUserStatusRequest(false),
                currentUserId: 0,
                CancellationToken.None
            );

        (
            await _handle
                .Db.RefreshTokens.AsNoTracking()
                .Where(rt => rt.UserId == user.UserId)
                .ToListAsync()
        ).ShouldAllBe(rt => rt.IsRevoked && rt.RevokedAt != null);
        (
            await _handle
                .Db.RefreshTokens.AsNoTracking()
                .Where(rt => rt.UserId == bystander.UserId)
                .ToListAsync()
        ).ShouldAllBe(rt => !rt.IsRevoked);
    }

    [Fact]
    public async Task UpdateUserStatusAsync_Activate_DoesNotTouchRefreshTokens()
    {
        var user = await SeedUserWithTokensAsync();

        await CreateSut()
            .UpdateUserStatusAsync(
                user.UserId,
                new UpdateUserStatusRequest(true),
                currentUserId: 0,
                CancellationToken.None
            );

        (
            await _handle
                .Db.RefreshTokens.AsNoTracking()
                .Where(rt => rt.UserId == user.UserId)
                .ToListAsync()
        ).ShouldAllBe(rt => !rt.IsRevoked);
    }

    private async Task<User> SeedUserWithTokensAsyncWithEmail(string email)
    {
        var user = TestDataSeeder.AddCustomer(_handle.Db, email: email);
        _handle.Db.RefreshTokens.Add(
            new RefreshToken
            {
                UserId = user.UserId,
                TokenHash = $"h-{email}",
                ExpiresAt = DateTime.UtcNow.AddDays(5),
                CreatedAt = DateTime.UtcNow,
            }
        );
        await _handle.Db.SaveChangesAsync();
        return user;
    }

    public void Dispose() => _handle.Dispose();
}
