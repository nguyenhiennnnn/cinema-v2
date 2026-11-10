import { apiGet } from "@/api/axios";
import type { ApiResponse } from "@/types/api";
import type { GenreDto, LanguageDto, AgeRatingDto, MembershipTierDto } from "@/features/admin/catalog/types";
import { ENDPOINTS } from "@/api/endpoints";

export const catalogApi = {
  getGenres: () =>
    apiGet<ApiResponse<GenreDto[]>>(ENDPOINTS.catalog.genres),

  getLanguages: () =>
    apiGet<ApiResponse<LanguageDto[]>>(ENDPOINTS.catalog.languages),

  getAgeRatings: () =>
    apiGet<ApiResponse<AgeRatingDto[]>>(ENDPOINTS.catalog.ageRatings),

  getMembershipTiers: () =>
    apiGet<ApiResponse<MembershipTierDto[]>>(ENDPOINTS.catalog.membershipTiers),
};
