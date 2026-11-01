export const Gender = {
  Male: "Male",
  Female: "Female",
  Other: "Other",
} as const;
export type Gender = (typeof Gender)[keyof typeof Gender];

export const MovieStatus = {
  ComingSoon: "ComingSoon",
  NowShowing: "NowShowing",
  Ended: "Ended",
} as const;
export type MovieStatus = (typeof MovieStatus)[keyof typeof MovieStatus];

export const ShowtimeStatus = {
  Scheduled: "Scheduled",
  Cancelled: "Cancelled",
  Completed: "Completed",
} as const;
export type ShowtimeStatus =
  (typeof ShowtimeStatus)[keyof typeof ShowtimeStatus];

export const ShowtimeSeatStatus = {
  Available: "Available",
  Held: "Held",
  Booked: "Booked",
} as const;
export type ShowtimeSeatStatus =
  (typeof ShowtimeSeatStatus)[keyof typeof ShowtimeSeatStatus];

export const SeatGapRuleMode = {
  Strict: "Strict",
  AllowEdge: "AllowEdge",
  Off: "Off",
} as const;
export type SeatGapRuleMode =
  (typeof SeatGapRuleMode)[keyof typeof SeatGapRuleMode];

export const BookingChannel = {
  Online: "Online",
  Counter: "Counter",
} as const;
export type BookingChannel =
  (typeof BookingChannel)[keyof typeof BookingChannel];

export const BookingStatus = {
  Pending: "Pending",
  Confirmed: "Confirmed",
  Cancelled: "Cancelled",
  Expired: "Expired",
} as const;
export type BookingStatus = (typeof BookingStatus)[keyof typeof BookingStatus];

export const PaymentMethod = {
  VNPay: "VNPay",
  Momo: "Momo",
  Cash: "Cash",
  Stripe: "Stripe",
} as const;
export type PaymentMethod = (typeof PaymentMethod)[keyof typeof PaymentMethod];

export const PaymentStatus = {
  Pending: "Pending",
  Success: "Success",
  Failed: "Failed",
  Refunded: "Refunded",
} as const;
export type PaymentStatus = (typeof PaymentStatus)[keyof typeof PaymentStatus];

export const VerificationTokenPurpose = {
  EmailConfirmation: "EmailConfirmation",
  PasswordReset: "PasswordReset",
} as const;
export type VerificationTokenPurpose =
  (typeof VerificationTokenPurpose)[keyof typeof VerificationTokenPurpose];

export const Role = {
  Customer: "Customer",
  Admin: "Admin",
  Employee: "Employee",
} as const;
export type Role = (typeof Role)[keyof typeof Role];

export const ReportGroupBy = {
  Day: "day",
  Week: "week",
  Month: "month",
} as const;
export type ReportGroupBy = (typeof ReportGroupBy)[keyof typeof ReportGroupBy];

export const DayOfWeek = {
  Mon: "Mon",
  Tue: "Tue",
  Wed: "Wed",
  Thu: "Thu",
  Fri: "Fri",
  Sat: "Sat",
  Sun: "Sun",
} as const;
export type DayOfWeek = (typeof DayOfWeek)[keyof typeof DayOfWeek];
