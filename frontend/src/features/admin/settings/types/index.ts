import type { SeatGapRuleMode } from "@/types/enum";

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
  timeZoneId?: string;
  locale?: string;
  currencyCode?: string;
  currencySymbol?: string;
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

export type SettingsDto = CinemaSettingsDto;
