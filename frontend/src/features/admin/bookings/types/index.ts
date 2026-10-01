import type { BookingStatus } from "@/types/enum";
import type {
  BookingMovieBriefDto,
  BookingShowtimeBriefDto,
  BookingSeatDto,
  BookingComboItemDto,
  BookingTicketDto,
  BookingPaymentDto,
} from "@/features/client/bookings/types";

export type {
  BookingMovieBriefDto,
  BookingShowtimeBriefDto,
  BookingSeatDto,
  BookingComboItemDto,
  BookingTicketDto,
  BookingPaymentDto,
};

// ── Admin booking list ────────────────────────────────────────────────────────

export interface AdminBookingListParams {
  status?: string;
  channel?: string;
  fromDate?: string;
  toDate?: string;
  search?: string;
  page?: number;
  pageSize?: number;
}

export interface AdminBookingListItemDto {
  bookingId: number;
  bookingCode: string;
  customerName: string;
  guestPhone?: string | null;
  finalAmount: number;
  status: BookingStatus;
  channel: string;
  createdAt: string;
}

// ── Admin booking detail ──────────────────────────────────────────────────────

export interface BookingEmployeeBriefDto {
  employeeId: number;
  fullName: string;
}

export interface AdminBookingDetailDto {
  bookingId: number;
  bookingCode: string;
  status: BookingStatus;
  movie: BookingMovieBriefDto;
  showtime: BookingShowtimeBriefDto;
  seats: BookingSeatDto[];
  combos: BookingComboItemDto[];
  totalAmount: number;
  discountAmount: number;
  finalAmount: number;
  createdAt: string;
  channel: string;
  createdByEmployee?: BookingEmployeeBriefDto | null;
  tickets?: BookingTicketDto[] | null;
  payments: BookingPaymentDto[];
}

// ── Counter hold ──────────────────────────────────────────────────────────────

export interface CounterHoldRequest {
  showtimeId: number;
  seatIds: number[];
  combos?: { comboId: number; quantity: number }[] | null;
  userId?: number | null;
  customerName: string;
  customerPhone?: string | null;
}

export interface CounterHoldResponse {
  bookingId: number;
  bookingCode: string;
  status: string;
  totalAmount: number;
  discountAmount: number;
  finalAmount: number;
  expiresAt: string;
  channel: string;
  createdByEmployeeId?: number | null;
}

// ── Counter checkout ──────────────────────────────────────────────────────────

export interface CounterCheckoutRequest {
  amountReceived: number;
}

export interface CounterCheckoutResponse {
  bookingId: number;
  status: string;
  change: number;
}

// ── Checkin ───────────────────────────────────────────────────────────────────

export interface CheckinRequest {
  qrCode: string;
}

export interface CheckinResponse {
  ticketId: number;
  bookingId: number;
  seatRow: string;
  seatNumber: number;
  movieTitle: string;
  startTime: string;
  checkedInAt: string;
}
