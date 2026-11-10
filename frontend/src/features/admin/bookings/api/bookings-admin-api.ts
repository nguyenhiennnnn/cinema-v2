import { apiGet, apiPost } from "@/api/axios";
import type { ApiResponse, OffsetPageResult } from "@/types/api";
import type {
  AdminBookingListParams,
  AdminBookingListItemDto,
  AdminBookingDetailDto,
  CounterHoldRequest,
  CounterHoldResponse,
  CounterCheckoutRequest,
  CounterCheckoutResponse,
  CheckinRequest,
  CheckinResponse,
} from "@/features/admin/bookings/types";
import { ENDPOINTS } from "@/api/endpoints";

export const bookingsAdminApi = {
  getBookings: (params: AdminBookingListParams) =>
    apiGet<ApiResponse<OffsetPageResult<AdminBookingListItemDto>>>(ENDPOINTS.admin.bookings.root, { params }),

  getBookingDetail: (bookingId: number) =>
    apiGet<ApiResponse<AdminBookingDetailDto>>(ENDPOINTS.admin.bookings.byId(bookingId)),

  counterHold: (body: CounterHoldRequest) =>
    apiPost<ApiResponse<CounterHoldResponse>>(ENDPOINTS.admin.bookings.counterHold, body),

  counterCheckout: (bookingId: number, body: CounterCheckoutRequest) =>
    apiPost<ApiResponse<CounterCheckoutResponse>>(ENDPOINTS.admin.bookings.counterCheckout(bookingId), body),

  checkin: (body: CheckinRequest) =>
    apiPost<ApiResponse<CheckinResponse>>(ENDPOINTS.admin.bookings.checkin, body),
};
