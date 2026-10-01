import { apiGet, apiPut } from "@/api/axios";
import type { ApiResponse, OffsetPageResult } from "@/types/api";
import type {
  AdminUserListParams,
  AdminUserListItemDto,
  AdminUserSelectDto,
  AdminUserSelectParams,
  UpdateUserStatusRequest,
  UpdateUserStatusResponse,
  UpdateUserRoleRequest,
  UpdateUserRoleResponse,
} from "../types";

const BASE = "/admin/users";

export const usersAdminApi = {
  getUsers: (params: AdminUserListParams) =>
    apiGet<ApiResponse<OffsetPageResult<AdminUserListItemDto>>>(BASE, { params }),

  getAdminUsers: (params?: AdminUserSelectParams) =>
    apiGet<ApiResponse<AdminUserSelectDto[]>>(`${BASE}/admins`, { params }),

  updateStatus: (id: number, body: UpdateUserStatusRequest) =>
    apiPut<ApiResponse<UpdateUserStatusResponse>>(`${BASE}/${id}/status`, body),

  updateRole: (id: number, body: UpdateUserRoleRequest) =>
    apiPut<ApiResponse<UpdateUserRoleResponse>>(`${BASE}/${id}/role`, body),
};
