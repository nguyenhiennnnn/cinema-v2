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
  QuickCreatePersonRequest,
  PositionDto, UpsertPositionRequest,
  ScreenTypeDto, UpsertScreenTypeRequest,
  SeatTypeDto, UpsertSeatTypeRequest,
  DeleteCategoryResponse,
  CatalogListParams,
  PersonListParams,
} from "../types";
import { ENDPOINTS } from "@/api/endpoints";

export const genresAdminApi = {
  getAll: (params?: CatalogListParams) =>
    apiGet<ApiResponse<OffsetPageResult<GenreDto>>>(ENDPOINTS.admin.genres.root, { params }),
  create: (body: UpsertGenreRequest) =>
    apiPost<ApiResponse<GenreDto>>(ENDPOINTS.admin.genres.root, body),
  update: (id: number, body: UpsertGenreRequest) =>
    apiPut<ApiResponse<GenreDto>>(ENDPOINTS.admin.genres.byId(id), body),
  delete: (id: number) =>
    apiDelete<ApiResponse<DeleteCategoryResponse>>(ENDPOINTS.admin.genres.byId(id)),
};

export const languagesAdminApi = {
  getAll: (params?: CatalogListParams) =>
    apiGet<ApiResponse<OffsetPageResult<LanguageDto>>>(ENDPOINTS.admin.languages.root, { params }),
  create: (body: UpsertLanguageRequest) =>
    apiPost<ApiResponse<LanguageDto>>(ENDPOINTS.admin.languages.root, body),
  update: (id: number, body: UpsertLanguageRequest) =>
    apiPut<ApiResponse<LanguageDto>>(ENDPOINTS.admin.languages.byId(id), body),
  delete: (id: number) =>
    apiDelete<ApiResponse<DeleteCategoryResponse>>(ENDPOINTS.admin.languages.byId(id)),
};

export const ageRatingsAdminApi = {
  getAll: (params?: CatalogListParams) =>
    apiGet<ApiResponse<OffsetPageResult<AgeRatingDto>>>(ENDPOINTS.admin.ageRatings.root, { params }),
  create: (body: UpsertAgeRatingRequest) =>
    apiPost<ApiResponse<AgeRatingDto>>(ENDPOINTS.admin.ageRatings.root, body),
  update: (id: number, body: UpsertAgeRatingRequest) =>
    apiPut<ApiResponse<AgeRatingDto>>(ENDPOINTS.admin.ageRatings.byId(id), body),
  delete: (id: number) =>
    apiDelete<ApiResponse<DeleteCategoryResponse>>(ENDPOINTS.admin.ageRatings.byId(id)),
};

export const membershipTiersAdminApi = {
  getAll: (params?: CatalogListParams) =>
    apiGet<ApiResponse<OffsetPageResult<MembershipTierDto>>>(ENDPOINTS.admin.membershipTiers.root, { params }),
  create: (body: UpsertMembershipTierRequest) =>
    apiPost<ApiResponse<MembershipTierDto>>(ENDPOINTS.admin.membershipTiers.root, body),
  update: (id: number, body: UpsertMembershipTierRequest) =>
    apiPut<ApiResponse<MembershipTierDto>>(ENDPOINTS.admin.membershipTiers.byId(id), body),
  delete: (id: number) =>
    apiDelete<ApiResponse<DeleteCategoryResponse>>(ENDPOINTS.admin.membershipTiers.byId(id)),
};

export const actorsAdminApi = {
  getAll: (params?: PersonListParams) =>
    apiGet<ApiResponse<OffsetPageResult<ActorDto>>>(ENDPOINTS.admin.actors.root, { params }),
  create: (body: UpsertPersonRequest) =>
    apiPost<ApiResponse<ActorDto>>(ENDPOINTS.admin.actors.root, body),
  quickCreate: (body: QuickCreatePersonRequest) =>
    apiPost<ApiResponse<ActorDto>>(ENDPOINTS.admin.actors.quick, body),
  update: (id: number, body: UpsertPersonRequest) =>
    apiPut<ApiResponse<ActorDto>>(ENDPOINTS.admin.actors.byId(id), body),
  delete: (id: number) =>
    apiDelete<ApiResponse<DeleteCategoryResponse>>(ENDPOINTS.admin.actors.byId(id)),
  bulkDelete: (body: BulkIdsRequest) =>
    apiDelete<ApiResponse<BulkActionResponse>>(ENDPOINTS.admin.actors.bulk, { data: body }),
};

export const directorsAdminApi = {
  getAll: (params?: PersonListParams) =>
    apiGet<ApiResponse<OffsetPageResult<DirectorDto>>>(ENDPOINTS.admin.directors.root, { params }),
  create: (body: UpsertPersonRequest) =>
    apiPost<ApiResponse<DirectorDto>>(ENDPOINTS.admin.directors.root, body),
  quickCreate: (body: QuickCreatePersonRequest) =>
    apiPost<ApiResponse<DirectorDto>>(ENDPOINTS.admin.directors.quick, body),
  update: (id: number, body: UpsertPersonRequest) =>
    apiPut<ApiResponse<DirectorDto>>(ENDPOINTS.admin.directors.byId(id), body),
  delete: (id: number) =>
    apiDelete<ApiResponse<DeleteCategoryResponse>>(ENDPOINTS.admin.directors.byId(id)),
  bulkDelete: (body: BulkIdsRequest) =>
    apiDelete<ApiResponse<BulkActionResponse>>(ENDPOINTS.admin.directors.bulk, { data: body }),
};

export const positionsAdminApi = {
  getAll: (params?: CatalogListParams) =>
    apiGet<ApiResponse<OffsetPageResult<PositionDto>>>(ENDPOINTS.admin.positions.root, { params }),
  create: (body: UpsertPositionRequest) =>
    apiPost<ApiResponse<PositionDto>>(ENDPOINTS.admin.positions.root, body),
  update: (id: number, body: UpsertPositionRequest) =>
    apiPut<ApiResponse<PositionDto>>(ENDPOINTS.admin.positions.byId(id), body),
  delete: (id: number) =>
    apiDelete<ApiResponse<DeleteCategoryResponse>>(ENDPOINTS.admin.positions.byId(id)),
};

export const screenTypesAdminApi = {
  getAll: (params?: CatalogListParams) =>
    apiGet<ApiResponse<OffsetPageResult<ScreenTypeDto>>>(ENDPOINTS.admin.screenTypes.root, { params }),
  create: (body: UpsertScreenTypeRequest) =>
    apiPost<ApiResponse<ScreenTypeDto>>(ENDPOINTS.admin.screenTypes.root, body),
  update: (id: number, body: UpsertScreenTypeRequest) =>
    apiPut<ApiResponse<ScreenTypeDto>>(ENDPOINTS.admin.screenTypes.byId(id), body),
  delete: (id: number) =>
    apiDelete<ApiResponse<DeleteCategoryResponse>>(ENDPOINTS.admin.screenTypes.byId(id)),
};

export const seatTypesAdminApi = {
  getAll: (params?: CatalogListParams) =>
    apiGet<ApiResponse<OffsetPageResult<SeatTypeDto>>>(ENDPOINTS.admin.seatTypes.root, { params }),
  create: (body: UpsertSeatTypeRequest) =>
    apiPost<ApiResponse<SeatTypeDto>>(ENDPOINTS.admin.seatTypes.root, body),
  update: (id: number, body: UpsertSeatTypeRequest) =>
    apiPut<ApiResponse<SeatTypeDto>>(ENDPOINTS.admin.seatTypes.byId(id), body),
  delete: (id: number) =>
    apiDelete<ApiResponse<DeleteCategoryResponse>>(ENDPOINTS.admin.seatTypes.byId(id)),
};
