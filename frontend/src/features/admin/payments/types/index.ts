export interface AdminPaymentListParams {
  status?: string;
  paymentMethod?: string;
  fromDate?: string;
  toDate?: string;
  search?: string;
  page?: number;
  pageSize?: number;
}

export interface AdminPaymentListItemDto {
  paymentId: number;
  bookingId: number;
  bookingCode: string;
  paymentMethod: string;
  transactionId?: string | null;
  gatewayResponseCode?: string | null;
  amount: number;
  status: string;
  paidAt?: string | null;
  createdAt: string;
}

export interface AdminPaymentBookingBriefDto {
  bookingId: number;
  bookingCode: string;
  status: string;
  finalAmount: number;
}

export interface AdminPaymentDetailDto {
  paymentId: number;
  bookingId: number;
  paymentMethod: string;
  transactionId?: string | null;
  gatewayResponseCode?: string | null;
  amount: number;
  status: string;
  paidAt?: string | null;
  createdAt: string;
  amountReceived?: number | null;
  changeGiven?: number | null;
  refundTransactionId?: string | null;
  refundedAmount?: number | null;
  refundedAt?: string | null;
  booking: AdminPaymentBookingBriefDto;
}
