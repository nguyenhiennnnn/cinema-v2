import type { BookingStatus } from "@/types/enum";

export interface ComboSelectionRequest {
  comboId: number;
  quantity: number;
}

export interface HoldSeatsRequest {
  showtimeId: number;
  customerName: string;
  customerPhone?: string | null;
  seatIds: number[];
  combos?: ComboSelectionRequest[] | null;
}

export interface HoldBookingResponse {
  bookingId: number;
  bookingCode: string;
  status: string;
  totalAmount: number;
  discountAmount: number;
  finalAmount: number;
  expiresAt: string;
}

export interface CheckoutRequest {
  paymentMethod: string;
}

export interface CheckoutResponse {
  paymentUrl: string;
  paymentId: number;
}

export interface CancelBookingResponse {
  bookingId: number;
  status: string;
  refundAmount?: number | null;
  refundStatus?: string | null;
}

export interface BookingListParams {
  status?: string;
  cursor?: string;
  limit?: number;
}

export interface BookingListItemDto {
  bookingId: number;
  bookingCode: string;
  movieTitle: string;
  startTime: string;
  finalAmount: number;
  status: BookingStatus;
  createdAt: string;
}

export interface BookingMovieBriefDto {
  movieId: number;
  title: string;
  posterUrl?: string | null;
}

export interface BookingShowtimeBriefDto {
  showtimeId: number;
  roomName: string;
  startTime: string;
}

export interface BookingSeatDto {
  seatRow: string;
  seatNumber: number;
  price: number;
}

export interface PricedSeat {
  seatRow: string;
  seatNumber: number;
  price: number;
}

export interface BookingComboItemDto {
  name: string;
  quantity: number;
  unitPrice: number;
}

export interface BookingTicketDto {
  ticketId: number;
  seatRow: string;
  seatNumber: number;
  qrCodeImage: string;
  isCheckedIn: boolean;
  checkedInAt?: string | null;
}

export interface BookingPaymentDto {
  paymentId: number;
  paymentMethod: string;
  status: string;
  amount: number;
  transactionId?: string | null;
  paidAt?: string | null;
  createdAt: string;
  refundedAmount?: number | null;
  refundedAt?: string | null;
}

export interface BookingDetailDto {
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
  tickets: BookingTicketDto[];
  payments: BookingPaymentDto[];
  holdExpiresAt?: string | null;
}
