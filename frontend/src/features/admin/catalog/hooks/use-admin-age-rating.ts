import { useMutation, useQuery } from "@tanstack/react-query";
import type { CatalogListParams, UpsertAgeRatingRequest } from "../types";
import { queryKeys } from "@/api/query-keys";
import { queryClient } from "@/api/query-client";
import { ageRatingsAdminApi } from "../api/catalog-api";

export function useAdminAgeRatings(params?: CatalogListParams) {
  return useQuery({
    queryKey: queryKeys.adminCatalog.ageRatings(params),
    queryFn: () => ageRatingsAdminApi.getAll(params),
    select: (res) => res.data,
  });
}

export function useAdminCreateAgeRating() {
  return useMutation({
    mutationFn: (body: UpsertAgeRatingRequest) =>
      ageRatingsAdminApi.create(body),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminCatalog.ageRatings(),
      }),
  });
}

export function useAdminUpdateAgeRating() {
  return useMutation({
    mutationFn: ({ id, body }: { id: number; body: UpsertAgeRatingRequest }) =>
      ageRatingsAdminApi.update(id, body),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminCatalog.ageRatings(),
      }),
  });
}

export function useAdminDeleteAgeRating() {
  return useMutation({
    mutationFn: (id: number) => ageRatingsAdminApi.delete(id),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminCatalog.ageRatings(),
      }),
  });
}
