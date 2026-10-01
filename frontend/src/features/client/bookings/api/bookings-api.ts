import { apiGet, apiPost } from "@/api/axios";
import type { ApiResponse, CursorPageResult } from "@/types/api";
import type {
  HoldSeatsRequest,
  HoldBookingResponse,
  CheckoutRequest,
  CheckoutResponse,
  CancelBookingResponse,
  BookingListParams,
  BookingListItemDto,
  BookingDetailDto,
} from "../types";

const BASE = "/bookings";

export const bookingsApi = {
  holdSeats: (body: HoldSeatsRequest) =>
    apiPost<ApiResponse<HoldBookingResponse>>(`${BASE}/hold`, body),

  getMyBookings: (params: BookingListParams) =>
    apiGet<ApiResponse<CursorPageResult<BookingListItemDto>>>(BASE, { params }),

  getBookingDetail: (bookingId: number) =>
    apiGet<ApiResponse<BookingDetailDto>>(`${BASE}/${bookingId}`),

  checkout: (bookingId: number, body: CheckoutRequest) =>
    apiPost<ApiResponse<CheckoutResponse>>(`${BASE}/${bookingId}/checkout`, body),

  cancelBooking: (bookingId: number) =>
    apiPost<ApiResponse<CancelBookingResponse>>(`${BASE}/${bookingId}/cancel`),
};
