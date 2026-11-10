import { apiGet, apiPut } from "@/api/axios";
import type { ApiResponse, OffsetPageResult } from "@/types/api";
import type { BulkActionResponse, BulkStatusRequest } from "@/types/bulk";
import type {
  AdminUserListParams,
  AdminUserListItemDto,
  AdminUserSelectDto,
  AdminUserSelectParams,
  UpdateUserStatusRequest,
  UpdateUserStatusResponse,
  UpdateUserRoleRequest,
  UpdateUserRoleResponse,
  CustomerLookupItemDto,
  CustomerLookupParams,
} from "../types";
import { ENDPOINTS } from "@/api/endpoints";

export const usersAdminApi = {
  getUsers: (params: AdminUserListParams) =>
    apiGet<ApiResponse<OffsetPageResult<AdminUserListItemDto>>>(ENDPOINTS.admin.users.root, { params }),

  getAdminUsers: (params?: AdminUserSelectParams) =>
    apiGet<ApiResponse<AdminUserSelectDto[]>>(ENDPOINTS.admin.users.admins, { params }),

  searchCustomers: (params: CustomerLookupParams) =>
    apiGet<ApiResponse<CustomerLookupItemDto[]>>(ENDPOINTS.admin.users.customers, { params }),

  updateStatus: (id: number, body: UpdateUserStatusRequest) =>
    apiPut<ApiResponse<UpdateUserStatusResponse>>(ENDPOINTS.admin.users.status(id), body),

  bulkUpdateStatus: (body: BulkStatusRequest) =>
    apiPut<ApiResponse<BulkActionResponse>>(ENDPOINTS.admin.users.bulkStatus, body),

  updateRole: (id: number, body: UpdateUserRoleRequest) =>
    apiPut<ApiResponse<UpdateUserRoleResponse>>(ENDPOINTS.admin.users.role(id), body),
};
