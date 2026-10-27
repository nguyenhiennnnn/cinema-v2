import { apiGet } from "@/api/axios";
import type { ApiResponse } from "@/types/api";
import type { ComboDto } from "@/features/admin/combos/types";
import type { ComboSuggestionDto } from "../types";

export const combosApi = {
  getActiveCombos: () => apiGet<ApiResponse<ComboDto[]>>("/combos"),

  getSuggestions: (showtimeId: number, seatCount: number) =>
    apiGet<ApiResponse<ComboSuggestionDto[]>>("/combos/suggestions", {
      params: { showtimeId, seatCount },
    }),
};
