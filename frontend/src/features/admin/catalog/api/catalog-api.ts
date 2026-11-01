import { apiDelete, apiGet, apiPost, apiPut } from "@/api/axios";
import type { ApiResponse, OffsetPageResult } from "@/types/api";
import type { BulkActionResponse, BulkIdsRequest } from "@/types/bulk";
import type {
  GenreDto, UpsertGenreRequest,
  LanguageDto, UpsertLanguageRequest,
  AgeRatingDto, UpsertAgeRatingRequest,
  MembershipTierDto, UpsertMembershipTierRequest,
  ActorDto,
  DirectorDto,
  UpsertPersonRequest,
  PositionDto, UpsertPositionRequest,
  ScreenTypeDto, UpsertScreenTypeRequest,
  SeatTypeDto, UpsertSeatTypeRequest,
  DeleteCategoryResponse,
  CatalogListParams,
  PersonListParams,
} from "../types";

export const genresAdminApi = {
  getAll: (params?: CatalogListParams) =>
    apiGet<ApiResponse<OffsetPageResult<GenreDto>>>("/admin/genres", { params }),
  create: (body: UpsertGenreRequest) =>
    apiPost<ApiResponse<GenreDto>>("/admin/genres", body),
  update: (id: number, body: UpsertGenreRequest) =>
    apiPut<ApiResponse<GenreDto>>(`/admin/genres/${id}`, body),
  delete: (id: number) =>
    apiDelete<ApiResponse<DeleteCategoryResponse>>(`/admin/genres/${id}`),
};

export const languagesAdminApi = {
  getAll: (params?: CatalogListParams) =>
    apiGet<ApiResponse<OffsetPageResult<LanguageDto>>>("/admin/languages", { params }),
  create: (body: UpsertLanguageRequest) =>
    apiPost<ApiResponse<LanguageDto>>("/admin/languages", body),
  update: (id: number, body: UpsertLanguageRequest) =>
    apiPut<ApiResponse<LanguageDto>>(`/admin/languages/${id}`, body),
  delete: (id: number) =>
    apiDelete<ApiResponse<DeleteCategoryResponse>>(`/admin/languages/${id}`),
};

export const ageRatingsAdminApi = {
  getAll: (params?: CatalogListParams) =>
    apiGet<ApiResponse<OffsetPageResult<AgeRatingDto>>>("/admin/age-ratings", { params }),
  create: (body: UpsertAgeRatingRequest) =>
    apiPost<ApiResponse<AgeRatingDto>>("/admin/age-ratings", body),
  update: (id: number, body: UpsertAgeRatingRequest) =>
    apiPut<ApiResponse<AgeRatingDto>>(`/admin/age-ratings/${id}`, body),
  delete: (id: number) =>
    apiDelete<ApiResponse<DeleteCategoryResponse>>(`/admin/age-ratings/${id}`),
};

export const membershipTiersAdminApi = {
  getAll: (params?: CatalogListParams) =>
    apiGet<ApiResponse<OffsetPageResult<MembershipTierDto>>>("/admin/membership-tiers", { params }),
  create: (body: UpsertMembershipTierRequest) =>
    apiPost<ApiResponse<MembershipTierDto>>("/admin/membership-tiers", body),
  update: (id: number, body: UpsertMembershipTierRequest) =>
    apiPut<ApiResponse<MembershipTierDto>>(`/admin/membership-tiers/${id}`, body),
  delete: (id: number) =>
    apiDelete<ApiResponse<DeleteCategoryResponse>>(`/admin/membership-tiers/${id}`),
};

export const actorsAdminApi = {
  getAll: (params?: PersonListParams) =>
    apiGet<ApiResponse<OffsetPageResult<ActorDto>>>("/admin/actors", { params }),
  create: (body: UpsertPersonRequest) =>
    apiPost<ApiResponse<ActorDto>>("/admin/actors", body),
  update: (id: number, body: UpsertPersonRequest) =>
    apiPut<ApiResponse<ActorDto>>(`/admin/actors/${id}`, body),
  delete: (id: number) =>
    apiDelete<ApiResponse<DeleteCategoryResponse>>(`/admin/actors/${id}`),
  bulkDelete: (body: BulkIdsRequest) =>
    apiDelete<ApiResponse<BulkActionResponse>>("/admin/actors/bulk", { data: body }),
};

export const directorsAdminApi = {
  getAll: (params?: PersonListParams) =>
    apiGet<ApiResponse<OffsetPageResult<DirectorDto>>>("/admin/directors", { params }),
  create: (body: UpsertPersonRequest) =>
    apiPost<ApiResponse<DirectorDto>>("/admin/directors", body),
  update: (id: number, body: UpsertPersonRequest) =>
    apiPut<ApiResponse<DirectorDto>>(`/admin/directors/${id}`, body),
  delete: (id: number) =>
    apiDelete<ApiResponse<DeleteCategoryResponse>>(`/admin/directors/${id}`),
  bulkDelete: (body: BulkIdsRequest) =>
    apiDelete<ApiResponse<BulkActionResponse>>("/admin/directors/bulk", { data: body }),
};

export const positionsAdminApi = {
  getAll: (params?: CatalogListParams) =>
    apiGet<ApiResponse<OffsetPageResult<PositionDto>>>("/admin/positions", { params }),
  create: (body: UpsertPositionRequest) =>
    apiPost<ApiResponse<PositionDto>>("/admin/positions", body),
  update: (id: number, body: UpsertPositionRequest) =>
    apiPut<ApiResponse<PositionDto>>(`/admin/positions/${id}`, body),
  delete: (id: number) =>
    apiDelete<ApiResponse<DeleteCategoryResponse>>(`/admin/positions/${id}`),
};

export const screenTypesAdminApi = {
  getAll: (params?: CatalogListParams) =>
    apiGet<ApiResponse<OffsetPageResult<ScreenTypeDto>>>("/admin/screen-types", { params }),
  create: (body: UpsertScreenTypeRequest) =>
    apiPost<ApiResponse<ScreenTypeDto>>("/admin/screen-types", body),
  update: (id: number, body: UpsertScreenTypeRequest) =>
    apiPut<ApiResponse<ScreenTypeDto>>(`/admin/screen-types/${id}`, body),
  delete: (id: number) =>
    apiDelete<ApiResponse<DeleteCategoryResponse>>(`/admin/screen-types/${id}`),
};

export const seatTypesAdminApi = {
  getAll: (params?: CatalogListParams) =>
    apiGet<ApiResponse<OffsetPageResult<SeatTypeDto>>>("/admin/seat-types", { params }),
  create: (body: UpsertSeatTypeRequest) =>
    apiPost<ApiResponse<SeatTypeDto>>("/admin/seat-types", body),
  update: (id: number, body: UpsertSeatTypeRequest) =>
    apiPut<ApiResponse<SeatTypeDto>>(`/admin/seat-types/${id}`, body),
  delete: (id: number) =>
    apiDelete<ApiResponse<DeleteCategoryResponse>>(`/admin/seat-types/${id}`),
};
