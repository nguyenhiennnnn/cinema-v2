import { useMutation, useQuery } from "@tanstack/react-query";
import { queryKeys } from "@/api/query-keys";
import { queryClient } from "@/api/query-client";
import type { CatalogListParams, UpsertGenreRequest } from "../types";
import { genresAdminApi } from "../api/catalog-api";

export function useAdminGenres(params?: CatalogListParams) {
  return useQuery({
    queryKey: queryKeys.adminCatalog.genres(params),
    queryFn: () => genresAdminApi.getAll(params),
    select: (res) => res.data,
  });
}

export function useAdminCreateGenre() {
  return useMutation({
    mutationFn: (body: UpsertGenreRequest) => genresAdminApi.create(body),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminCatalog.genres(),
      }),
  });
}

export function useAdminUpdateGenre() {
  return useMutation({
    mutationFn: ({ id, body }: { id: number; body: UpsertGenreRequest }) =>
      genresAdminApi.update(id, body),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminCatalog.genres(),
      }),
  });
}

export function useAdminDeleteGenre() {
  return useMutation({
    mutationFn: (id: number) => genresAdminApi.delete(id),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminCatalog.genres(),
      }),
  });
}
