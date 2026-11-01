using Cinema.Application.DTOs.Catalog;
using Cinema.Application.DTOs.Common;

namespace Cinema.Application.Interfaces;

public interface ICatalogService
{
    Task<List<GenreDto>> GetGenresAsync(CancellationToken cancellationToken);
    Task<List<LanguageDto>> GetLanguagesAsync(CancellationToken cancellationToken);
    Task<List<AgeRatingDto>> GetAgeRatingsAsync(CancellationToken cancellationToken);
    Task<List<MembershipTierDto>> GetMembershipTiersAsync(CancellationToken cancellationToken);

    Task<OffsetPageResult<GenreDto>> GetGenresAdminAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );
    Task<GenreDto> CreateGenreAsync(
        UpsertGenreRequest request,
        CancellationToken cancellationToken
    );
    Task<GenreDto> UpdateGenreAsync(
        int id,
        UpsertGenreRequest request,
        CancellationToken cancellationToken
    );
    Task<DeleteCategoryResponse> DeleteGenreAsync(int id, CancellationToken cancellationToken);

    Task<OffsetPageResult<LanguageDto>> GetLanguagesAdminAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );
    Task<LanguageDto> CreateLanguageAsync(
        UpsertLanguageRequest request,
        CancellationToken cancellationToken
    );
    Task<LanguageDto> UpdateLanguageAsync(
        int id,
        UpsertLanguageRequest request,
        CancellationToken cancellationToken
    );
    Task<DeleteCategoryResponse> DeleteLanguageAsync(int id, CancellationToken cancellationToken);

    Task<OffsetPageResult<AgeRatingDto>> GetAgeRatingsAdminAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );
    Task<AgeRatingDto> CreateAgeRatingAsync(
        UpsertAgeRatingRequest request,
        CancellationToken cancellationToken
    );
    Task<AgeRatingDto> UpdateAgeRatingAsync(
        int id,
        UpsertAgeRatingRequest request,
        CancellationToken cancellationToken
    );
    Task<DeleteCategoryResponse> DeleteAgeRatingAsync(int id, CancellationToken cancellationToken);

    Task<OffsetPageResult<DirectorDto>> GetDirectorsAdminAsync(
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );
    Task<DirectorDto> CreateDirectorAsync(
        UpsertPersonRequest request,
        CancellationToken cancellationToken
    );
    Task<DirectorDto> UpdateDirectorAsync(
        int id,
        UpsertPersonRequest request,
        CancellationToken cancellationToken
    );
    Task<DeleteCategoryResponse> DeleteDirectorAsync(int id, CancellationToken cancellationToken);
    Task<BulkActionResponse> BulkDeleteDirectorsAsync(
        BulkIdsRequest request,
        CancellationToken cancellationToken
    );

    Task<OffsetPageResult<ActorDto>> GetActorsAdminAsync(
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );
    Task<ActorDto> CreateActorAsync(
        UpsertPersonRequest request,
        CancellationToken cancellationToken
    );
    Task<ActorDto> UpdateActorAsync(
        int id,
        UpsertPersonRequest request,
        CancellationToken cancellationToken
    );
    Task<DeleteCategoryResponse> DeleteActorAsync(int id, CancellationToken cancellationToken);
    Task<BulkActionResponse> BulkDeleteActorsAsync(
        BulkIdsRequest request,
        CancellationToken cancellationToken
    );

    Task<OffsetPageResult<PositionDto>> GetPositionsAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );
    Task<PositionDto> CreatePositionAsync(
        UpsertPositionRequest request,
        CancellationToken cancellationToken
    );
    Task<PositionDto> UpdatePositionAsync(
        int id,
        UpsertPositionRequest request,
        CancellationToken cancellationToken
    );
    Task<DeleteCategoryResponse> DeletePositionAsync(int id, CancellationToken cancellationToken);

    Task<OffsetPageResult<ScreenTypeDto>> GetScreenTypesAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );
    Task<ScreenTypeDto> CreateScreenTypeAsync(
        UpsertScreenTypeRequest request,
        CancellationToken cancellationToken
    );
    Task<ScreenTypeDto> UpdateScreenTypeAsync(
        int id,
        UpsertScreenTypeRequest request,
        CancellationToken cancellationToken
    );
    Task<DeleteCategoryResponse> DeleteScreenTypeAsync(int id, CancellationToken cancellationToken);

    Task<OffsetPageResult<SeatTypeDto>> GetSeatTypesAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );
    Task<SeatTypeDto> CreateSeatTypeAsync(
        UpsertSeatTypeRequest request,
        CancellationToken cancellationToken
    );
    Task<SeatTypeDto> UpdateSeatTypeAsync(
        int id,
        UpsertSeatTypeRequest request,
        CancellationToken cancellationToken
    );
    Task<DeleteCategoryResponse> DeleteSeatTypeAsync(int id, CancellationToken cancellationToken);

    Task<OffsetPageResult<MembershipTierDto>> GetMembershipTiersAdminAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );
    Task<MembershipTierDto> CreateMembershipTierAsync(
        UpsertMembershipTierRequest request,
        CancellationToken cancellationToken
    );
    Task<MembershipTierDto> UpdateMembershipTierAsync(
        int id,
        UpsertMembershipTierRequest request,
        CancellationToken cancellationToken
    );
    Task<DeleteCategoryResponse> DeleteMembershipTierAsync(
        int id,
        CancellationToken cancellationToken
    );
}
