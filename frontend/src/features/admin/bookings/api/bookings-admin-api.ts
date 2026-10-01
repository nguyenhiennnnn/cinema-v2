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
} from "../types";

const BASE = "/admin/bookings";

export const bookingsAdminApi = {
  getBookings: (params: AdminBookingListParams) =>
    apiGet<ApiResponse<OffsetPageResult<AdminBookingListItemDto>>>(BASE, { params }),

  getBookingDetail: (bookingId: number) =>
    apiGet<ApiResponse<AdminBookingDetailDto>>(`${BASE}/${bookingId}`),

  counterHold: (body: CounterHoldRequest) =>
    apiPost<ApiResponse<CounterHoldResponse>>(`${BASE}/counter-hold`, body),

  counterCheckout: (bookingId: number, body: CounterCheckoutRequest) =>
    apiPost<ApiResponse<CounterCheckoutResponse>>(`${BASE}/${bookingId}/counter-checkout`, body),

  checkin: (body: CheckinRequest) =>
    apiPost<ApiResponse<CheckinResponse>>(`${BASE}/checkin`, body),
};
