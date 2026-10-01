import { apiGet } from "@/api/axios";
import type { ApiResponse } from "@/types/api";
import type { GenreDto, LanguageDto, AgeRatingDto, MembershipTierDto } from "@/features/admin/catalog/types";

export const catalogApi = {
  getGenres: () =>
    apiGet<ApiResponse<GenreDto[]>>("/genres"),

  getLanguages: () =>
    apiGet<ApiResponse<LanguageDto[]>>("/languages"),

  getAgeRatings: () =>
    apiGet<ApiResponse<AgeRatingDto[]>>("/age-ratings"),

  getMembershipTiers: () =>
    apiGet<ApiResponse<MembershipTierDto[]>>("/membership-tiers"),
};
