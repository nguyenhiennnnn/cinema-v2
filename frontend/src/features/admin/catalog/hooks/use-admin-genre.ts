import { keepPreviousData, useMutation, useQuery } from "@tanstack/react-query";
import { queryKeys } from "@/api/query-keys";
import type { CatalogListParams, UpsertGenreRequest } from "@/features/admin/catalog/types";
import { genresAdminApi } from "../api/catalog-api";

export function useAdminGenres(params?: CatalogListParams) {
  return useQuery({
    queryKey: queryKeys.adminCatalog.genres(params),
    queryFn: () => genresAdminApi.getAll(params),
    placeholderData: keepPreviousData,
    select: (res) => res.data,
  });
}

export function useAdminCreateGenre() {
  return useMutation({
    meta: { invalidates: "genreChanged" },
    mutationFn: (body: UpsertGenreRequest) => genresAdminApi.create(body),
  });
}

export function useAdminUpdateGenre() {
  return useMutation({
    meta: { invalidates: "genreChanged" },
    mutationFn: ({ id, body }: { id: number; body: UpsertGenreRequest }) =>
      genresAdminApi.update(id, body),
  });
}

export function useAdminDeleteGenre() {
  return useMutation({
    meta: { invalidates: "genreChanged" },
    mutationFn: (id: number) => genresAdminApi.delete(id),
  });
}
