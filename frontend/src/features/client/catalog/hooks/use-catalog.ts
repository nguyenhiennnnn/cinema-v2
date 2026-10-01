import { useQuery } from "@tanstack/react-query";
import { catalogApi } from "../api/catalog-api";
import { queryKeys } from "@/api/query-keys";

export function useGenres() {
  return useQuery({
    queryKey: queryKeys.catalog.genres(),
    queryFn: () => catalogApi.getGenres(),
    select: (res) => res.data,
  });
}

export function useLanguages() {
  return useQuery({
    queryKey: queryKeys.catalog.languages(),
    queryFn: () => catalogApi.getLanguages(),
    select: (res) => res.data,
  });
}

export function useAgeRatings() {
  return useQuery({
    queryKey: queryKeys.catalog.ageRatings(),
    queryFn: () => catalogApi.getAgeRatings(),
    select: (res) => res.data,
  });
}

export function useMembershipTiers() {
  return useQuery({
    queryKey: queryKeys.catalog.membershipTiers(),
    queryFn: () => catalogApi.getMembershipTiers(),
    select: (res) => res.data,
  });
}
