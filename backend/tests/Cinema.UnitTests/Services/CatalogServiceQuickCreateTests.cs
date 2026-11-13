using Cinema.Application.DTOs.Catalog;
using Cinema.Application.Services.Catalog;
using Cinema.Shared.Exceptions;
using Cinema.UnitTests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class CatalogServiceQuickCreateTests(PostgresContainerFixture fixture) : IDisposable, IClassFixture<PostgresContainerFixture>
{
    private readonly PostgresAppDbContextHandle _handle = new(fixture.MasterConnectionString);

    private CatalogService CreateSut()
    {
        _handle.Db.ChangeTracker.Clear();
        return new CatalogService(_handle.Db);
    }

    public void Dispose() => _handle.Dispose();

    [Fact]
    public async Task CreateGenreAsync_NameHasExtraSpaces_SavesNormalizedName()
    {
        var sut = CreateSut();

        var result = await sut.CreateGenreAsync(
            new UpsertGenreRequest("  Thể   loại mới  "),
            CancellationToken.None
        );

        result.Name.ShouldBe("Thể loại mới");
    }

    [Fact]
    public async Task CreateGenreAsync_SeededNameInDifferentCaseAndSpacing_ThrowsConflict()
    {
        var sut = CreateSut();

        await Should.ThrowAsync<ConflictAppException>(() =>
            sut.CreateGenreAsync(new UpsertGenreRequest("  kinh   DỊ "), CancellationToken.None)
        );
    }

    [Fact]
    public async Task CreateScreenTypeAsync_SeededNameInDifferentCase_ThrowsConflict()
    {
        var sut = CreateSut();

        await Should.ThrowAsync<ConflictAppException>(() =>
            sut.CreateScreenTypeAsync(new UpsertScreenTypeRequest("imax"), CancellationToken.None)
        );
    }

    [Fact]
    public async Task CreateAgeRatingAsync_CodeDiffersByCase_ThrowsConflict()
    {
        var sut = CreateSut();
        await sut.CreateAgeRatingAsync(
            new UpsertAgeRatingRequest("X99", null),
            CancellationToken.None
        );

        await Should.ThrowAsync<ConflictAppException>(() =>
            sut.CreateAgeRatingAsync(
                new UpsertAgeRatingRequest("x99", null),
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task QuickCreateActorAsync_NewName_CreatesNormalizedRecord()
    {
        var sut = CreateSut();

        var result = await sut.QuickCreateActorAsync(
            new QuickCreatePersonRequest("  Zed   Testman "),
            CancellationToken.None
        );

        result.FullName.ShouldBe("Zed Testman");
        result.ActorId.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task QuickCreateActorAsync_ExistingNameInDifferentCase_ReturnsExistingRecord()
    {
        var sut = CreateSut();
        var first = await sut.QuickCreateActorAsync(
            new QuickCreatePersonRequest("Zed Testman"),
            CancellationToken.None
        );

        var second = await sut.QuickCreateActorAsync(
            new QuickCreatePersonRequest(" zed  TESTMAN"),
            CancellationToken.None
        );

        second.ActorId.ShouldBe(first.ActorId);
        (await _handle.Db.Actors.CountAsync(a => a.FullName == "Zed Testman")).ShouldBe(1);
    }

    [Fact]
    public async Task QuickCreateDirectorAsync_SeededNameInDifferentCase_ReturnsSeededRecord()
    {
        var sut = CreateSut();

        var result = await sut.QuickCreateDirectorAsync(
            new QuickCreatePersonRequest("christopher   nolan"),
            CancellationToken.None
        );

        result.DirectorId.ShouldBe(1);
        (await _handle.Db.Directors.CountAsync(d => d.FullName == "Christopher Nolan")).ShouldBe(1);
    }

    [Fact]
    public async Task GetActorsAdminAsync_SearchIgnoresCase()
    {
        var sut = CreateSut();
        await sut.QuickCreateActorAsync(
            new QuickCreatePersonRequest("Zed Testman"),
            CancellationToken.None
        );

        var result = await sut.GetActorsAdminAsync("ZED TESTMAN", 1, 20, CancellationToken.None);

        result.Items.Count.ShouldBe(1);
        result.Items[0].FullName.ShouldBe("Zed Testman");
    }
}
