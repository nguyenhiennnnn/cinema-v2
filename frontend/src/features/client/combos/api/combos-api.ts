import { apiGet } from "@/api/axios";
import type { ApiResponse } from "@/types/api";
import type { ComboDto } from "@/features/admin/combos/types";

export const combosApi = {
  getActiveCombos: () =>
    apiGet<ApiResponse<ComboDto[]>>("/combos"),
};
