// ── Combo ─────────────────────────────────────────────────────────────────────

export interface ComboDto {
  comboId: number;
  name: string;
  description?: string | null;
  price: number;
  imageUrl?: string | null;
}

export interface AdminComboDto {
  comboId: number;
  name: string;
  description?: string | null;
  price: number;
  imageUrl?: string | null;
  isActive: boolean;
}

export interface UpsertComboRequest {
  name: string;
  description?: string | null;
  price: number;
  imageUrl?: string | null;
}

export interface ToggleComboResponse {
  comboId: number;
  isActive: boolean;
}

export interface ComboListParams {
  page?: number;
  pageSize?: number;
}
