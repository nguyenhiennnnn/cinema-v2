import { useMutation, useQuery } from "@tanstack/react-query";
import type { CatalogListParams, UpsertAgeRatingRequest } from "../types";
import { queryKeys } from "@/api/query-keys";
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
    meta: { invalidates: "ageRatingChanged" },
    mutationFn: (body: UpsertAgeRatingRequest) =>
      ageRatingsAdminApi.create(body),
  });
}

export function useAdminUpdateAgeRating() {
  return useMutation({
    meta: { invalidates: "ageRatingChanged" },
    mutationFn: ({ id, body }: { id: number; body: UpsertAgeRatingRequest }) =>
      ageRatingsAdminApi.update(id, body),
  });
}

export function useAdminDeleteAgeRating() {
  return useMutation({
    meta: { invalidates: "ageRatingChanged" },
    mutationFn: (id: number) => ageRatingsAdminApi.delete(id),
  });
}
