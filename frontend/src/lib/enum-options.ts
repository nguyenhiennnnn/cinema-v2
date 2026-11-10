import {
  BookingStatus,
  MovieStatus,
  PaymentMethod,
  PaymentStatus,
  ShowtimeStatus,
} from "@/types/enum";

export interface EnumOption<T extends string = string> {
  value: T;
  label: string;
}

function toOptions<T extends Record<string, string>>(
  enumObj: T,
  labels: Record<T[keyof T], string>,
): EnumOption<T[keyof T]>[] {
  return Object.values(enumObj).map((value) => ({
    value: value as T[keyof T],
    label: labels[value as T[keyof T]],
  }));
}

export const roleOptions: EnumOption[] = [
  { value: "1", label: "Khách hàng" },
  { value: "2", label: "Quản trị viên" },
  { value: "3", label: "Nhân viên" },
];

export const bookingStatusOptions = toOptions(BookingStatus, {
  Pending: "Chờ xử lý",
  Confirmed: "Đã xác nhận",
  Cancelled: "Đã hủy",
  Expired: "Hết hạn",
});

export const paymentStatusOptions = toOptions(PaymentStatus, {
  Pending: "Chờ xử lý",
  Success: "Thành công",
  Failed: "Thất bại",
  Refunded: "Đã hoàn tiền",
});

export const paymentMethodOptions = toOptions(PaymentMethod, {
  VNPay: "VNPay",
  Momo: "Momo",
  Cash: "Tiền mặt",
  Stripe: "Stripe",
});

export const movieStatusOptions = toOptions(MovieStatus, {
  ComingSoon: "Sắp chiếu",
  NowShowing: "Đang chiếu",
  Ended: "Đã ngừng chiếu",
});

export const showtimeStatusOptions = toOptions(ShowtimeStatus, {
  Scheduled: "Đã lên lịch",
  Cancelled: "Đã hủy",
  Completed: "Đã hoàn thành",
});
