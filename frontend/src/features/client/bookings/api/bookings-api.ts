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
import { ENDPOINTS } from "@/api/endpoints";

export const bookingsApi = {
  holdSeats: (body: HoldSeatsRequest) =>
    apiPost<ApiResponse<HoldBookingResponse>>(ENDPOINTS.bookings.hold, body),

  getMyBookings: (params: BookingListParams) =>
    apiGet<ApiResponse<CursorPageResult<BookingListItemDto>>>(ENDPOINTS.bookings.root, { params }),

  getBookingDetail: (bookingId: number) =>
    apiGet<ApiResponse<BookingDetailDto>>(ENDPOINTS.bookings.byId(bookingId)),

  checkout: (bookingId: number, body: CheckoutRequest) =>
    apiPost<ApiResponse<CheckoutResponse>>(ENDPOINTS.bookings.checkout(bookingId), body),

  cancelBooking: (bookingId: number) =>
    apiPost<ApiResponse<CancelBookingResponse>>(ENDPOINTS.bookings.cancel(bookingId)),
};
