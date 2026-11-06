import type { SeatGapRuleMode } from "@/types/enum";

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
  holdDurationMinutes: number;
  seatGapRuleMode: SeatGapRuleMode;
  roomBufferMinutes: number;
  maxSeatsPerBooking: number;
  maxComboQuantity: number;
  loyaltyWindowMonths: number;
  movieEndGraceDays: number;
  showtimeLookaheadDays: number;
  schedulerStepMinutes: number;
  schedulerMinSameMovieGapMinutes: number;
  schedulerHeatLookbackDays: number;
  schedulerHeatSalesWeight: number;
  schedulerHeatRatingWeight: number;
  schedulerMaxRating: number;
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
  seatGapRuleMode: SeatGapRuleMode;
  roomBufferMinutes?: number;
  maxSeatsPerBooking?: number;
  maxComboQuantity?: number;
  loyaltyWindowMonths?: number;
  movieEndGraceDays?: number;
  showtimeLookaheadDays?: number;
  schedulerStepMinutes?: number;
  schedulerMinSameMovieGapMinutes?: number;
  schedulerHeatLookbackDays?: number;
  schedulerHeatSalesWeight?: number;
  schedulerHeatRatingWeight?: number;
  schedulerMaxRating?: number;
}

// Alias for backward compat
export type SettingsDto = CinemaSettingsDto;
