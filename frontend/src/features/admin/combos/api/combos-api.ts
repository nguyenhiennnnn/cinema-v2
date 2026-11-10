import { apiGet, apiPost, apiPut } from "@/api/axios";
import type { ApiResponse, OffsetPageResult } from "@/types/api";
import type { BulkActionResponse, BulkStatusRequest } from "@/types/bulk";
import type { AdminComboDto, UpsertComboRequest, ToggleComboResponse, ComboListParams } from "../types";
import { ENDPOINTS } from "@/api/endpoints";

export const combosAdminApi = {
  getCombos: (params?: ComboListParams) =>
    apiGet<ApiResponse<OffsetPageResult<AdminComboDto>>>(ENDPOINTS.admin.combos.root, { params }),

  createCombo: (body: UpsertComboRequest) =>
    apiPost<ApiResponse<AdminComboDto>>(ENDPOINTS.admin.combos.root, body),

  updateCombo: (id: number, body: UpsertComboRequest) =>
    apiPut<ApiResponse<AdminComboDto>>(ENDPOINTS.admin.combos.byId(id), body),

  toggleCombo: (id: number) =>
    apiPut<ApiResponse<ToggleComboResponse>>(ENDPOINTS.admin.combos.toggle(id)),

  bulkSetActive: (body: BulkStatusRequest) =>
    apiPut<ApiResponse<BulkActionResponse>>(ENDPOINTS.admin.combos.bulkStatus, body),
};
