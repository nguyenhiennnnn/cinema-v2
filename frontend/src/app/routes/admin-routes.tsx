import type { RouteObject } from "react-router";

import AdminLayout from "@/layouts/admin/admin-layout";
import AdminAuthLayout from "@/layouts/admin/admin-auth-layout";
import {
  AdminIndexRedirect,
  AdminProtectedRoute,
} from "./protected-route/admin-protected-route";
import { GuestRoute } from "./protected-route/guest-route";
import { ROUTES } from "@/config/routes";
import { lazyRoute } from "./lazy-route";

const AdminLoginPage = lazyRoute(() => import("@/features/admin/login/pages/login-page"));
const AdminResetPasswordPage = lazyRoute(() => import("@/features/admin/login/pages/reset-password-page"));
const AdminChangePasswordPage = lazyRoute(() => import("@/features/admin/account/pages/change-password-page"));
const AdminDashboardPage = lazyRoute(() => import("@/features/admin/dashboard/pages/dashboard-page"));
const AdminShowtimesPage = lazyRoute(() => import("@/features/admin/showtimes/pages/showtimes-page"));
const AdminCounterPage = lazyRoute(() => import("@/features/admin/counter/pages/counter-page"));
const AdminCheckinPage = lazyRoute(() => import("@/features/admin/checkin/pages/checkin-page"));
const AdminPaymentsPage = lazyRoute(() => import("@/features/admin/payments/pages/payments-page"));
const AdminUsersPage = lazyRoute(() => import("@/features/admin/users/pages/users-page"));
const AdminReportsPage = lazyRoute(() => import("@/features/admin/reports/pages/reports.page"));
const AdminRoomsPage = lazyRoute(() => import("@/features/admin/rooms/pages/rooms-page"));
const AdminEmployeesPage = lazyRoute(() => import("@/features/admin/employees/pages/employees-page"));
const AdminRoomSeatsPage = lazyRoute(() => import("@/features/admin/rooms/pages/room-seats-page"));
const AdminCombosPage = lazyRoute(() => import("@/features/admin/combos/pages/combos-page"));
const AdminReviewsPage = lazyRoute(() => import("@/features/admin/reviews/pages/reviews-page"));
const AdminImportExportHistoryPage = lazyRoute(() => import("@/features/admin/import-export/pages/import-export-history-page"));
const AdminSettingsPage = lazyRoute(() => import("@/features/admin/settings/pages/settings-page"));
const CatalogHubPage = lazyRoute(() => import("@/features/admin/catalog/pages/catalog-hub-page"));
const GenresPage = lazyRoute(() => import("@/features/admin/catalog/pages/genres-page"));
const LanguagesPage = lazyRoute(() => import("@/features/admin/catalog/pages/languages-page"));
const AgeRatingsPage = lazyRoute(() => import("@/features/admin/catalog/pages/age-ratings-page"));
const SeatTypesPage = lazyRoute(() => import("@/features/admin/catalog/pages/seat-types-page"));
const ScreenTypesPage = lazyRoute(() => import("@/features/admin/catalog/pages/screen-types-page"));
const MembershipTiersPage = lazyRoute(() => import("@/features/admin/catalog/pages/membership-tiers-page"));
const PositionsPage = lazyRoute(() => import("@/features/admin/catalog/pages/positions-page"));
const ActorsPage = lazyRoute(() => import("@/features/admin/catalog/pages/actors-page"));
const DirectorsPage = lazyRoute(() => import("@/features/admin/catalog/pages/directors-page"));
const AdminMoviesPage = lazyRoute(() => import("@/features/admin/movies/pages/movies-page"));
const AdminBookingsPage = lazyRoute(() => import("@/features/admin/bookings/pages/bookings-page"));
const AdminBookingDetailPage = lazyRoute(() => import("@/features/admin/bookings/pages/booking-detail-page"));

export const adminRoutes: RouteObject[] = [
  {
    path: ROUTES.admin.login,
    element: <GuestRoute redirectTo={ROUTES.admin.dashboard} />,
    children: [
      {
        element: <AdminAuthLayout />,
        children: [{ index: true, lazy: AdminLoginPage }],
      },
    ],
  },
  {
    path: ROUTES.admin.resetPassword,
    element: <AdminAuthLayout />,
    children: [{ index: true, lazy: AdminResetPasswordPage }],
  },
  {
    path: "/admin",
    element: <AdminProtectedRoute />,
    children: [
      { index: true, element: <AdminIndexRedirect /> },
      {
        path: "change-password",
        element: <AdminAuthLayout />,
        children: [{ index: true, lazy: AdminChangePasswordPage }],
      },
      {
        element: <AdminLayout />,
        children: [
          { path: "dashboard", lazy: AdminDashboardPage },
          { path: "movies", lazy: AdminMoviesPage },
          { path: "showtimes", lazy: AdminShowtimesPage },
          { path: "bookings", lazy: AdminBookingsPage },
          { path: "bookings/:id", lazy: AdminBookingDetailPage },
          { path: "bookings/counter", lazy: AdminCounterPage },
          { path: "checkin", lazy: AdminCheckinPage },
          { path: "payments", lazy: AdminPaymentsPage },
          { path: "reports", lazy: AdminReportsPage },
          { path: "users", lazy: AdminUsersPage },
          { path: "employees", lazy: AdminEmployeesPage },
          { path: "rooms", lazy: AdminRoomsPage },
          { path: "rooms/:id/seats", lazy: AdminRoomSeatsPage },
          { path: "combos", lazy: AdminCombosPage },
          { path: "reviews", lazy: AdminReviewsPage },
          { path: "import-export", lazy: AdminImportExportHistoryPage },
          { path: "settings", lazy: AdminSettingsPage },
          { path: "catalog", lazy: CatalogHubPage },
          { path: "catalog/genres", lazy: GenresPage },
          { path: "catalog/languages", lazy: LanguagesPage },
          { path: "catalog/age-ratings", lazy: AgeRatingsPage },
          { path: "catalog/seat-types", lazy: SeatTypesPage },
          { path: "catalog/screen-types", lazy: ScreenTypesPage },
          {
            path: "catalog/membership-tiers",
            lazy: MembershipTiersPage,
          },
          { path: "catalog/positions", lazy: PositionsPage },
          { path: "catalog/actors", lazy: ActorsPage },
          { path: "catalog/directors", lazy: DirectorsPage },
        ],
      },
    ],
  },
];
