// ── Admin user list ───────────────────────────────────────────────────────────
// Backend: GetUsers([FromQuery] int? roleId, [FromQuery] string? search, page, pageSize)

export interface AdminUserListParams {
  roleId?: number;
  search?: string;
  page?: number;
  pageSize?: number;
}

export interface AdminUserListItemDto {
  userId: number;
  fullName: string;
  email: string;
  roleId: string;
  roleName: string;
  isActive: boolean;
  createdAt: string;
}

export interface AdminUserSelectDto {
  userId: number;
  fullName: string;
  email: string;
}

export interface AdminUserSelectParams {
  search?: string;
  availableOnly?: boolean;
}

// ── Update status / role ──────────────────────────────────────────────────────

export interface UpdateUserStatusRequest {
  isActive: boolean;
}

export interface UpdateUserStatusResponse {
  userId: number;
  isActive: boolean;
}

export interface UpdateUserRoleRequest {
  roleId: number;
}

export interface UpdateUserRoleResponse {
  userId: number;
  roleId: number;
  roleName: string;
}
