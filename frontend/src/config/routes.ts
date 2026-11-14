import { Role } from "@/types/enum";

export const ROUTES = {
  home: "/",
  forbidden: "/403",
  schedule: "/schedule",
  membership: "/membership",
  policy: (slug: string) => `/policies/${slug}`,
  movies: {
    list: "/movies",
    detail: (id: number | string) => `/movies/${id}`,
  },
  booking: {
    root: "/booking",
    detail: (showtimeId: number | string | undefined) =>
      `/booking/${showtimeId}`,
  },
  auth: {
    root: "/auth",
    login: "/auth/login",
    register: "/auth/register",
    forgotPassword: "/auth/forgot-password",
  },
  account: {
    root: "/account",
    ticketDetail: (bookingId: number | string) =>
      `/account/tickets/${bookingId}`,
    profile: "/account/profile",
    tickets: "/account/tickets",
    reviews: "/account/reviews",
    changePassword: "/account/change-password",
  },
  admin: {
    root: "/admin",
    login: "/admin/login",
    resetPassword: "/admin/reset-password",
    changePassword: "/admin/change-password",
    dashboard: "/admin/dashboard",
    bookingCounter: "/admin/bookings/counter",
    checkin: "/admin/checkin",
    movies: "/admin/movies",
    showtimes: "/admin/showtimes",
    rooms: "/admin/rooms",
    combos: "/admin/combos",
    bookings: "/admin/bookings",
    payments: "/admin/payments",
    reports: "/admin/reports",
    users: "/admin/users",
    employees: "/admin/employees",
    reviews: "/admin/reviews",
    catalog: "/admin/catalog",
    catalogGenres: "/admin/catalog/genres",
    catalogLanguages: "/admin/catalog/languages",
    catalogAgeRatings: "/admin/catalog/age-ratings",
    catalogSeatTypes: "/admin/catalog/seat-types",
    catalogScreenTypes: "/admin/catalog/screen-types",
    catalogMembershipTiers: "/admin/catalog/membership-tiers",
    catalogPositions: "/admin/catalog/positions",
    catalogActors: "/admin/catalog/actors",
    catalogDirectors: "/admin/catalog/directors",
    bookingDetail: (bookingId: number | string) =>
      `/admin/bookings/${bookingId}`,
    roomSeats: (roomId: number | string) => `/admin/rooms/${roomId}/seats`,
    settings: "/admin/settings",
  },
} as const;

export const getAdminHomePath = (role?: string | null) =>
  role === Role.Employee ? ROUTES.admin.bookingCounter : ROUTES.admin.dashboard;
