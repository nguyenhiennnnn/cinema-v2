import { useQuery } from "@tanstack/react-query";
import { combosApi } from "../api/combos-api";
import { queryKeys } from "@/api/query-keys";

export function useActiveCombos() {
  return useQuery({
    queryKey: queryKeys.combos.active(),
    queryFn: () => combosApi.getActiveCombos(),
    select: (res) => res.data,
  });
}
