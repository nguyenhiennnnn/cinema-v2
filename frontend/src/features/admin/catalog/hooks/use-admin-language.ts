import { useMutation, useQuery } from "@tanstack/react-query";
import type { CatalogListParams, UpsertLanguageRequest } from "../types";
import { queryKeys } from "@/api/query-keys";
import { languagesAdminApi } from "../api/catalog-api";
import { queryClient } from "@/api/query-client";

export function useAdminLanguages(params?: CatalogListParams) {
  return useQuery({
    queryKey: queryKeys.adminCatalog.languages(params),
    queryFn: () => languagesAdminApi.getAll(params),
    select: (res) => res.data,
  });
}

export function useAdminCreateLanguage() {
  return useMutation({
    mutationFn: (body: UpsertLanguageRequest) => languagesAdminApi.create(body),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminCatalog.languages(),
      }),
  });
}

export function useAdminUpdateLanguage() {
  return useMutation({
    mutationFn: ({ id, body }: { id: number; body: UpsertLanguageRequest }) =>
      languagesAdminApi.update(id, body),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminCatalog.languages(),
      }),
  });
}

export function useAdminDeleteLanguage() {
  return useMutation({
    mutationFn: (id: number) => languagesAdminApi.delete(id),
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: queryKeys.adminCatalog.languages(),
      }),
  });
}
