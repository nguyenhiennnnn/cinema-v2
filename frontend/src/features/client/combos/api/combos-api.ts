import { apiGet } from "@/api/axios";
import type { ApiResponse } from "@/types/api";
import type { ComboDto } from "@/features/admin/combos/types";
import type { ComboSuggestionDto } from "../types";
import { ENDPOINTS } from "@/api/endpoints";

export const combosApi = {
  getActiveCombos: () => apiGet<ApiResponse<ComboDto[]>>(ENDPOINTS.combos.root),

  getSuggestions: (showtimeId: number, seatCount: number) =>
    apiGet<ApiResponse<ComboSuggestionDto[]>>(ENDPOINTS.combos.suggestions, {
      params: { showtimeId, seatCount },
    }),
};
