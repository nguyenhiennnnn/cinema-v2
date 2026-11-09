// ── Catalog DTOs (match backend Cinema.Application.DTOs.Catalog exactly) ──────

export interface GenreDto {
  genreId: number;
  name: string;
}

export interface LanguageDto {
  languageId: number;
  name: string;
}

export interface AgeRatingDto {
  ageRatingId: number;
  code: string;
  description?: string | null;
}

export interface MembershipTierDto {
  tierId: number;
  tierName: string;
  minPoints: number;
  discountPercent: number;
}

export interface DirectorDto {
  directorId: number;
  fullName: string;
  avatarUrl?: string | null;
}

export interface ActorDto {
  actorId: number;
  fullName: string;
  avatarUrl?: string | null;
}

export interface PositionDto {
  positionId: number;
  name: string;
}

export interface ScreenTypeDto {
  screenTypeId: number;
  name: string;
}

export interface SeatTypeDto {
  seatTypeId: number;
  name: string;
  priceMultiplier: number;
}

// ── Upsert requests ───────────────────────────────────────────────────────────

export interface UpsertGenreRequest {
  name: string;
}
export interface UpsertLanguageRequest {
  name: string;
}
export interface UpsertPositionRequest {
  name: string;
}
export interface UpsertScreenTypeRequest {
  name: string;
}

export interface UpsertAgeRatingRequest {
  code: string;
  description?: string | null;
}

export interface UpsertPersonRequest {
  fullName: string;
  avatarUrl?: string | null;
}

export interface QuickCreatePersonRequest {
  fullName: string;
}

export interface UpsertSeatTypeRequest {
  name: string;
  priceMultiplier: number;
}

export interface UpsertMembershipTierRequest {
  tierName: string;
  minPoints: number;
  discountPercent: number;
}

export interface DeleteCategoryResponse {
  id: number;
  deleted: boolean;
}

// ── Shared list params ────────────────────────────────────────────────────────

export interface CatalogListParams {
  page?: number;
  pageSize?: number;
}

export interface PersonListParams extends CatalogListParams {
  search?: string;
}
