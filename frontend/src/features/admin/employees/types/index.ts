// ── Employee ──────────────────────────────────────────────────────────────────
// Backend: GetEmployees([FromQuery] int? positionId, page, pageSize)

export interface AdminEmployeeListParams {
  positionId?: number;
  page?: number;
  pageSize?: number;
}

export interface AdminEmployeeDto {
  employeeId: number;
  fullName: string;
  email: string;
  positionId?: number | null;
  positionName?: string | null;
  hireDate?: string | null;
  isActive: boolean;
}

export interface CreateEmployeeRequest {
  userId: number;
  positionId?: number | null;
  hireDate?: string | null;
}

export interface UpdateEmployeeRequest {
  positionId?: number | null;
  hireDate?: string | null;
  isActive: boolean;
}
