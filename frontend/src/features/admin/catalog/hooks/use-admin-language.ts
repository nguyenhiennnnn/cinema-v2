import { useMutation, useQuery } from "@tanstack/react-query";
import type { CatalogListParams, UpsertLanguageRequest } from "../types";
import { queryKeys } from "@/api/query-keys";
import { languagesAdminApi } from "../api/catalog-api";

export function useAdminLanguages(params?: CatalogListParams) {
  return useQuery({
    queryKey: queryKeys.adminCatalog.languages(params),
    queryFn: () => languagesAdminApi.getAll(params),
    select: (res) => res.data,
  });
}

export function useAdminCreateLanguage() {
  return useMutation({
    meta: { invalidates: "languageChanged" },
    mutationFn: (body: UpsertLanguageRequest) => languagesAdminApi.create(body),
  });
}

export function useAdminUpdateLanguage() {
  return useMutation({
    meta: { invalidates: "languageChanged" },
    mutationFn: ({ id, body }: { id: number; body: UpsertLanguageRequest }) =>
      languagesAdminApi.update(id, body),
  });
}

export function useAdminDeleteLanguage() {
  return useMutation({
    meta: { invalidates: "languageChanged" },
    mutationFn: (id: number) => languagesAdminApi.delete(id),
  });
}
