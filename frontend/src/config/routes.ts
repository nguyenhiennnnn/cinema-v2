import { Role } from "@/types/enum";

export const ROUTES = {
  home: "/",
  auth: {
    login: "/auth/login",
    register: "/auth/register",
  },
  account: {
    profile: "/account/profile",
    tickets: "/account/tickets",
    reviews: "/account/reviews",
    changePassword: "/account/change-password",
  },
  admin: {
    login: "/admin/login",
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
    settings: "/admin/settings",
  },
} as const;

export const getAdminHomePath = (role?: string | null) =>
  role === Role.Employee ? ROUTES.admin.bookingCounter : ROUTES.admin.dashboard;
