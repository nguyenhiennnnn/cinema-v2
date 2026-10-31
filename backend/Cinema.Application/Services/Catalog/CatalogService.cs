using Cinema.Application.DTOs.Catalog;
using Cinema.Application.DTOs.Common;
using Cinema.Application.Interfaces;
using Cinema.Domain.Entities;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Services.Catalog;

public class CatalogService(IAppDbContext db) : ICatalogService
{
    public async Task<List<GenreDto>> GetGenresAsync(CancellationToken cancellationToken) =>
        await db
            .Genres.OrderBy(g => g.Name)
            .ProjectToType<GenreDto>()
            .ToListAsync(cancellationToken);

    public async Task<List<LanguageDto>> GetLanguagesAsync(CancellationToken cancellationToken) =>
        await db
            .Languages.OrderBy(l => l.Name)
            .ProjectToType<LanguageDto>()
            .ToListAsync(cancellationToken);

    public async Task<List<AgeRatingDto>> GetAgeRatingsAsync(CancellationToken cancellationToken) =>
        await db
            .AgeRatings.OrderBy(a => a.Code)
            .ProjectToType<AgeRatingDto>()
            .ToListAsync(cancellationToken);

    public async Task<List<MembershipTierDto>> GetMembershipTiersAsync(
        CancellationToken cancellationToken
    ) =>
        await db
            .MembershipTiers.OrderBy(t => t.MinPoints)
            .ProjectToType<MembershipTierDto>()
            .ToListAsync(cancellationToken);

    public async Task<OffsetPageResult<GenreDto>> GetGenresAdminAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = db.Genres.OrderBy(g => g.Name);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<GenreDto>()
            .ToListAsync(cancellationToken);
        return OffsetPageResult<GenreDto>.Create(items, page, pageSize, total);
    }

    public async Task<GenreDto> CreateGenreAsync(
        UpsertGenreRequest request,
        CancellationToken cancellationToken
    )
    {
        var exists = await db.Genres.AnyAsync(g => g.Name == request.Name, cancellationToken);
        if (exists)
            throw new ConflictAppException(Messages.Conflict_GenreExists);

        var entity = new Genre { Name = request.Name };
        db.Genres.Add(entity);
        await db.SaveChangesAsync(cancellationToken);
        return entity.Adapt<GenreDto>();
    }

    public async Task<GenreDto> UpdateGenreAsync(
        int id,
        UpsertGenreRequest request,
        CancellationToken cancellationToken
    )
    {
        var entity =
            await db.Genres.FirstOrDefaultAsync(g => g.GenreId == id, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_Genre);
        entity.Name = request.Name;
        await db.SaveChangesAsync(cancellationToken);
        return entity.Adapt<GenreDto>();
    }

    public async Task<DeleteCategoryResponse> DeleteGenreAsync(
        int id,
        CancellationToken cancellationToken
    )
    {
        var entity =
            await db.Genres.FirstOrDefaultAsync(g => g.GenreId == id, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_Genre);

        var isReferenced = await db.MovieGenres.AnyAsync(mg => mg.GenreId == id, cancellationToken);
        if (isReferenced)
            throw new ConflictAppException(Messages.Conflict_GenreInUse);

        db.Genres.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
        return new DeleteCategoryResponse(id, true);
    }

    // ---------------- Languages (Admin) ----------------

    public async Task<OffsetPageResult<LanguageDto>> GetLanguagesAdminAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = db.Languages.OrderBy(l => l.Name);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<LanguageDto>()
            .ToListAsync(cancellationToken);
        return OffsetPageResult<LanguageDto>.Create(items, page, pageSize, total);
    }

    public async Task<LanguageDto> CreateLanguageAsync(
        UpsertLanguageRequest request,
        CancellationToken cancellationToken
    )
    {
        var exists = await db.Languages.AnyAsync(l => l.Name == request.Name, cancellationToken);
        if (exists)
            throw new ConflictAppException(Messages.Conflict_LanguageExists);

        var entity = new Language { Name = request.Name };
        db.Languages.Add(entity);
        await db.SaveChangesAsync(cancellationToken);
        return entity.Adapt<LanguageDto>();
    }

    public async Task<LanguageDto> UpdateLanguageAsync(
        int id,
        UpsertLanguageRequest request,
        CancellationToken cancellationToken
    )
    {
        var entity =
            await db.Languages.FirstOrDefaultAsync(l => l.LanguageId == id, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_Language);
        entity.Name = request.Name;
        await db.SaveChangesAsync(cancellationToken);
        return entity.Adapt<LanguageDto>();
    }

    public async Task<DeleteCategoryResponse> DeleteLanguageAsync(
        int id,
        CancellationToken cancellationToken
    )
    {
        var entity =
            await db.Languages.FirstOrDefaultAsync(l => l.LanguageId == id, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_Language);

        var isReferenced = await db.Movies.AnyAsync(m => m.LanguageId == id, cancellationToken);
        if (isReferenced)
            throw new ConflictAppException(Messages.Conflict_LanguageInUse);

        db.Languages.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
        return new DeleteCategoryResponse(id, true);
    }

    public async Task<OffsetPageResult<AgeRatingDto>> GetAgeRatingsAdminAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = db.AgeRatings.OrderBy(a => a.Code);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<AgeRatingDto>()
            .ToListAsync(cancellationToken);
        return OffsetPageResult<AgeRatingDto>.Create(items, page, pageSize, total);
    }

    public async Task<AgeRatingDto> CreateAgeRatingAsync(
        UpsertAgeRatingRequest request,
        CancellationToken cancellationToken
    )
    {
        var exists = await db.AgeRatings.AnyAsync(a => a.Code == request.Code, cancellationToken);
        if (exists)
            throw new ConflictAppException(Messages.Conflict_AgeRatingCodeExists);

        var entity = new AgeRating { Code = request.Code, Description = request.Description };
        db.AgeRatings.Add(entity);
        await db.SaveChangesAsync(cancellationToken);
        return entity.Adapt<AgeRatingDto>();
    }

    public async Task<AgeRatingDto> UpdateAgeRatingAsync(
        int id,
        UpsertAgeRatingRequest request,
        CancellationToken cancellationToken
    )
    {
        var entity =
            await db.AgeRatings.FirstOrDefaultAsync(a => a.AgeRatingId == id, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_AgeRating);
        entity.Code = request.Code;
        entity.Description = request.Description;
        await db.SaveChangesAsync(cancellationToken);
        return entity.Adapt<AgeRatingDto>();
    }

    public async Task<DeleteCategoryResponse> DeleteAgeRatingAsync(
        int id,
        CancellationToken cancellationToken
    )
    {
        var entity =
            await db.AgeRatings.FirstOrDefaultAsync(a => a.AgeRatingId == id, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_AgeRating);

        var isReferenced = await db.Movies.AnyAsync(m => m.AgeRatingId == id, cancellationToken);
        if (isReferenced)
            throw new ConflictAppException(
                Messages.Conflict_AgeRatingInUse
            );

        db.AgeRatings.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
        return new DeleteCategoryResponse(id, true);
    }

    public async Task<OffsetPageResult<DirectorDto>> GetDirectorsAdminAsync(
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = db.Directors.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(d => d.FullName.Contains(search));
        query = query.OrderBy(d => d.FullName);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<DirectorDto>()
            .ToListAsync(cancellationToken);
        return OffsetPageResult<DirectorDto>.Create(items, page, pageSize, total);
    }

    public async Task<DirectorDto> CreateDirectorAsync(
        UpsertPersonRequest request,
        CancellationToken cancellationToken
    )
    {
        var entity = new Director { FullName = request.FullName, AvatarUrl = request.AvatarUrl };
        db.Directors.Add(entity);
        await db.SaveChangesAsync(cancellationToken);
        return entity.Adapt<DirectorDto>();
    }

    public async Task<DirectorDto> UpdateDirectorAsync(
        int id,
        UpsertPersonRequest request,
        CancellationToken cancellationToken
    )
    {
        var entity =
            await db.Directors.FirstOrDefaultAsync(d => d.DirectorId == id, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_Director);
        entity.FullName = request.FullName;
        entity.AvatarUrl = request.AvatarUrl;
        await db.SaveChangesAsync(cancellationToken);
        return entity.Adapt<DirectorDto>();
    }

    public async Task<DeleteCategoryResponse> DeleteDirectorAsync(
        int id,
        CancellationToken cancellationToken
    )
    {
        var entity =
            await db.Directors.FirstOrDefaultAsync(d => d.DirectorId == id, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_Director);

        var isReferenced = await db.Movies.AnyAsync(m => m.DirectorId == id, cancellationToken);
        if (isReferenced)
            throw new ConflictAppException(Messages.Conflict_DirectorInUse);

        db.Directors.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
        return new DeleteCategoryResponse(id, true);
    }

    public async Task<OffsetPageResult<ActorDto>> GetActorsAdminAsync(
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = db.Actors.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(a => a.FullName.Contains(search));
        query = query.OrderBy(a => a.FullName);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<ActorDto>()
            .ToListAsync(cancellationToken);
        return OffsetPageResult<ActorDto>.Create(items, page, pageSize, total);
    }

    public async Task<ActorDto> CreateActorAsync(
        UpsertPersonRequest request,
        CancellationToken cancellationToken
    )
    {
        var entity = new Actor { FullName = request.FullName, AvatarUrl = request.AvatarUrl };
        db.Actors.Add(entity);
        await db.SaveChangesAsync(cancellationToken);
        return entity.Adapt<ActorDto>();
    }

    public async Task<ActorDto> UpdateActorAsync(
        int id,
        UpsertPersonRequest request,
        CancellationToken cancellationToken
    )
    {
        var entity =
            await db.Actors.FirstOrDefaultAsync(a => a.ActorId == id, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_Actor);
        entity.FullName = request.FullName;
        entity.AvatarUrl = request.AvatarUrl;
        await db.SaveChangesAsync(cancellationToken);
        return entity.Adapt<ActorDto>();
    }

    public async Task<DeleteCategoryResponse> DeleteActorAsync(
        int id,
        CancellationToken cancellationToken
    )
    {
        var entity =
            await db.Actors.FirstOrDefaultAsync(a => a.ActorId == id, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_Actor);

        var isReferenced = await db.MovieCast.AnyAsync(mc => mc.ActorId == id, cancellationToken);
        if (isReferenced)
            throw new ConflictAppException(
                Messages.Conflict_ActorInUse
            );

        db.Actors.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
        return new DeleteCategoryResponse(id, true);
    }

    public async Task<OffsetPageResult<PositionDto>> GetPositionsAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = db.Positions.OrderBy(p => p.Name);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<PositionDto>()
            .ToListAsync(cancellationToken);
        return OffsetPageResult<PositionDto>.Create(items, page, pageSize, total);
    }

    public async Task<PositionDto> CreatePositionAsync(
        UpsertPositionRequest request,
        CancellationToken cancellationToken
    )
    {
        var exists = await db.Positions.AnyAsync(p => p.Name == request.Name, cancellationToken);
        if (exists)
            throw new ConflictAppException(Messages.Conflict_PositionExists);

        var entity = new Position { Name = request.Name };
        db.Positions.Add(entity);
        await db.SaveChangesAsync(cancellationToken);
        return entity.Adapt<PositionDto>();
    }

    public async Task<PositionDto> UpdatePositionAsync(
        int id,
        UpsertPositionRequest request,
        CancellationToken cancellationToken
    )
    {
        var entity =
            await db.Positions.FirstOrDefaultAsync(p => p.PositionId == id, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_Position);
        entity.Name = request.Name;
        await db.SaveChangesAsync(cancellationToken);
        return entity.Adapt<PositionDto>();
    }

    public async Task<DeleteCategoryResponse> DeletePositionAsync(
        int id,
        CancellationToken cancellationToken
    )
    {
        var entity =
            await db.Positions.FirstOrDefaultAsync(p => p.PositionId == id, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_Position);

        var isReferenced = await db.Employees.AnyAsync(e => e.PositionId == id, cancellationToken);
        if (isReferenced)
            throw new ConflictAppException(Messages.Conflict_PositionInUse);

        db.Positions.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
        return new DeleteCategoryResponse(id, true);
    }

    public async Task<OffsetPageResult<ScreenTypeDto>> GetScreenTypesAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = db.ScreenTypes.OrderBy(s => s.Name);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<ScreenTypeDto>()
            .ToListAsync(cancellationToken);
        return OffsetPageResult<ScreenTypeDto>.Create(items, page, pageSize, total);
    }

    public async Task<ScreenTypeDto> CreateScreenTypeAsync(
        UpsertScreenTypeRequest request,
        CancellationToken cancellationToken
    )
    {
        var exists = await db.ScreenTypes.AnyAsync(s => s.Name == request.Name, cancellationToken);
        if (exists)
            throw new ConflictAppException(Messages.Conflict_ScreenTypeExists);

        var entity = new ScreenType { Name = request.Name };
        db.ScreenTypes.Add(entity);
        await db.SaveChangesAsync(cancellationToken);
        return entity.Adapt<ScreenTypeDto>();
    }

    public async Task<ScreenTypeDto> UpdateScreenTypeAsync(
        int id,
        UpsertScreenTypeRequest request,
        CancellationToken cancellationToken
    )
    {
        var entity =
            await db.ScreenTypes.FirstOrDefaultAsync(s => s.ScreenTypeId == id, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_ScreenType);
        entity.Name = request.Name;
        await db.SaveChangesAsync(cancellationToken);
        return entity.Adapt<ScreenTypeDto>();
    }

    public async Task<DeleteCategoryResponse> DeleteScreenTypeAsync(
        int id,
        CancellationToken cancellationToken
    )
    {
        var entity =
            await db.ScreenTypes.FirstOrDefaultAsync(s => s.ScreenTypeId == id, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_ScreenType);

        var isReferenced = await db.CinemaRooms.AnyAsync(
            r => r.ScreenTypeId == id,
            cancellationToken
        );
        if (isReferenced)
            throw new ConflictAppException(
                Messages.Conflict_ScreenTypeInUse
            );

        db.ScreenTypes.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
        return new DeleteCategoryResponse(id, true);
    }

    public async Task<OffsetPageResult<SeatTypeDto>> GetSeatTypesAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = db.SeatTypes.OrderBy(s => s.Name);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<SeatTypeDto>()
            .ToListAsync(cancellationToken);
        return OffsetPageResult<SeatTypeDto>.Create(items, page, pageSize, total);
    }

    public async Task<SeatTypeDto> CreateSeatTypeAsync(
        UpsertSeatTypeRequest request,
        CancellationToken cancellationToken
    )
    {
        var exists = await db.SeatTypes.AnyAsync(s => s.Name == request.Name, cancellationToken);
        if (exists)
            throw new ConflictAppException(Messages.Conflict_SeatTypeExists);

        var entity = new SeatType
        {
            Name = request.Name,
            PriceMultiplier = request.PriceMultiplier,
        };
        db.SeatTypes.Add(entity);
        await db.SaveChangesAsync(cancellationToken);
        return entity.Adapt<SeatTypeDto>();
    }

    public async Task<SeatTypeDto> UpdateSeatTypeAsync(
        int id,
        UpsertSeatTypeRequest request,
        CancellationToken cancellationToken
    )
    {
        var entity =
            await db.SeatTypes.FirstOrDefaultAsync(s => s.SeatTypeId == id, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_SeatType);
        entity.Name = request.Name;
        entity.PriceMultiplier = request.PriceMultiplier;
        await db.SaveChangesAsync(cancellationToken);
        return entity.Adapt<SeatTypeDto>();
    }

    public async Task<DeleteCategoryResponse> DeleteSeatTypeAsync(
        int id,
        CancellationToken cancellationToken
    )
    {
        var entity =
            await db.SeatTypes.FirstOrDefaultAsync(s => s.SeatTypeId == id, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_SeatType);

        var isReferenced = await db.Seats.AnyAsync(s => s.SeatTypeId == id, cancellationToken);
        if (isReferenced)
            throw new ConflictAppException(Messages.Conflict_SeatTypeInUse);

        db.SeatTypes.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
        return new DeleteCategoryResponse(id, true);
    }

    public async Task<OffsetPageResult<MembershipTierDto>> GetMembershipTiersAdminAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = db.MembershipTiers.OrderBy(t => t.MinPoints);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<MembershipTierDto>()
            .ToListAsync(cancellationToken);
        return OffsetPageResult<MembershipTierDto>.Create(items, page, pageSize, total);
    }

    public async Task<MembershipTierDto> CreateMembershipTierAsync(
        UpsertMembershipTierRequest request,
        CancellationToken cancellationToken
    )
    {
        var exists = await db.MembershipTiers.AnyAsync(
            t => t.TierName == request.TierName,
            cancellationToken
        );
        if (exists)
            throw new ConflictAppException(Messages.Conflict_MembershipTierExists);

        var entity = new MembershipTier
        {
            TierName = request.TierName,
            MinPoints = request.MinPoints,
            DiscountPercent = request.DiscountPercent,
        };
        db.MembershipTiers.Add(entity);
        await db.SaveChangesAsync(cancellationToken);
        return entity.Adapt<MembershipTierDto>();
    }

    public async Task<MembershipTierDto> UpdateMembershipTierAsync(
        int id,
        UpsertMembershipTierRequest request,
        CancellationToken cancellationToken
    )
    {
        var entity =
            await db.MembershipTiers.FirstOrDefaultAsync(t => t.TierId == id, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_MembershipTier);
        entity.TierName = request.TierName;
        entity.MinPoints = request.MinPoints;
        entity.DiscountPercent = request.DiscountPercent;
        await db.SaveChangesAsync(cancellationToken);
        return entity.Adapt<MembershipTierDto>();
    }

    public async Task<DeleteCategoryResponse> DeleteMembershipTierAsync(
        int id,
        CancellationToken cancellationToken
    )
    {
        var entity =
            await db.MembershipTiers.FirstOrDefaultAsync(t => t.TierId == id, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_MembershipTier);

        var isReferenced = await db.CustomerProfiles.AnyAsync(
            p => p.TierId == id,
            cancellationToken
        );
        if (isReferenced)
            throw new ConflictAppException(
                Messages.Conflict_MembershipTierInUse
            );

        db.MembershipTiers.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
        return new DeleteCategoryResponse(id, true);
    }
}
