import { apiGet, apiPost, apiPut } from "@/api/axios";
import type { ApiResponse, OffsetPageResult } from "@/types/api";
import type { AdminComboDto, UpsertComboRequest, ToggleComboResponse, ComboListParams } from "../types";

const BASE = "/admin/combos";

export const combosAdminApi = {
  getCombos: (params?: ComboListParams) =>
    apiGet<ApiResponse<OffsetPageResult<AdminComboDto>>>(BASE, { params }),

  createCombo: (body: UpsertComboRequest) =>
    apiPost<ApiResponse<AdminComboDto>>(BASE, body),

  updateCombo: (id: number, body: UpsertComboRequest) =>
    apiPut<ApiResponse<AdminComboDto>>(`${BASE}/${id}`, body),

  toggleCombo: (id: number) =>
    apiPut<ApiResponse<ToggleComboResponse>>(`${BASE}/${id}/toggle`),
};
