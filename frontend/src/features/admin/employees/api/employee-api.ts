import { apiGet, apiPost, apiPut } from "@/api/axios";
import type { ApiResponse, OffsetPageResult } from "@/types/api";
import type {
  AdminEmployeeListParams,
  AdminEmployeeDto,
  CreateEmployeeRequest,
  UpdateEmployeeRequest,
} from "../types";

const BASE = "/admin/employees";

export const employeesAdminApi = {
  getEmployees: (params?: AdminEmployeeListParams) =>
    apiGet<ApiResponse<OffsetPageResult<AdminEmployeeDto>>>(BASE, { params }),

  createEmployee: (body: CreateEmployeeRequest) =>
    apiPost<ApiResponse<AdminEmployeeDto>>(BASE, body),

  updateEmployee: (id: number, body: UpdateEmployeeRequest) =>
    apiPut<ApiResponse<AdminEmployeeDto>>(`${BASE}/${id}`, body),
};
