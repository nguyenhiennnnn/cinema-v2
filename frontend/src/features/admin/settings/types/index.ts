// Backend DTO is named CinemaSettingsDto, not SettingsDto

export interface CinemaSettingsDto {
  name: string;
  address: string;
  phone?: string | null;
  email?: string | null;
  taxCode?: string | null;
  logoUrl?: string | null;
  openingTime?: string | null;
  closingTime?: string | null;
  pointsPerAmount: number;
  cancelRefundPercent: number;
  cancelDeadlineHours: number;
  stopSellingMinutesBefore: number;
}

export interface UpdateSettingsRequest {
  name: string;
  address: string;
  phone?: string | null;
  email?: string | null;
  taxCode?: string | null;
  logoUrl?: string | null;
  openingTime?: string | null;
  closingTime?: string | null;
  pointsPerAmount: number;
  cancelRefundPercent: number;
  cancelDeadlineHours: number;
  stopSellingMinutesBefore: number;
}

// Alias for backward compat
export type SettingsDto = CinemaSettingsDto;
