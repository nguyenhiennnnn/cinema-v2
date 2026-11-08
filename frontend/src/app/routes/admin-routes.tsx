import { lazy } from "react";
import type { RouteObject } from "react-router";

import AdminLayout from "@/layouts/admin/admin-layout";
import AdminAuthLayout from "@/layouts/admin/admin-auth-layout";
import {
  AdminIndexRedirect,
  AdminProtectedRoute,
} from "./protected-route/admin-protected-route";
import { GuestRoute } from "./protected-route/guest-route";
import { ROUTES } from "@/config/routes";
import { LazyPage } from "./lazy-page";

const AdminLoginPage = lazy(() => import("@/features/admin/login/pages/login-page"));
const AdminDashboardPage = lazy(() => import("@/features/admin/dashboard/pages/dashboard-page"));
const AdminShowtimesPage = lazy(() => import("@/features/admin/showtimes/pages/showtimes-page"));
const AdminCounterPage = lazy(() => import("@/features/admin/counter/pages/counter-page"));
const AdminCheckinPage = lazy(() => import("@/features/admin/checkin/pages/checkin-page"));
const AdminPaymentsPage = lazy(() => import("@/features/admin/payments/pages/payments-page"));
const AdminUsersPage = lazy(() => import("@/features/admin/users/pages/users-page"));
const AdminReportsPage = lazy(() => import("@/features/admin/reports/pages/reports.page"));
const AdminRoomsPage = lazy(() => import("@/features/admin/rooms/pages/rooms-page"));
const AdminEmployeesPage = lazy(() => import("@/features/admin/employees/pages/employees-page"));
const AdminRoomSeatsPage = lazy(() => import("@/features/admin/rooms/pages/room-seats-page"));
const AdminCombosPage = lazy(() => import("@/features/admin/combos/pages/combos-page"));
const AdminReviewsPage = lazy(() => import("@/features/admin/reviews/pages/reviews-page"));
const AdminSettingsPage = lazy(() => import("@/features/admin/settings/pages/settings-page"));
const CatalogHubPage = lazy(() => import("@/features/admin/catalog/pages/catalog-hub-page"));
const GenresPage = lazy(() => import("@/features/admin/catalog/pages/genres-page"));
const LanguagesPage = lazy(() => import("@/features/admin/catalog/pages/languages-page"));
const AgeRatingsPage = lazy(() => import("@/features/admin/catalog/pages/age-ratings-page"));
const SeatTypesPage = lazy(() => import("@/features/admin/catalog/pages/seat-types-page"));
const ScreenTypesPage = lazy(() => import("@/features/admin/catalog/pages/screen-types-page"));
const MembershipTiersPage = lazy(() => import("@/features/admin/catalog/pages/membership-tiers-page"));
const PositionsPage = lazy(() => import("@/features/admin/catalog/pages/positions-page"));
const ActorsPage = lazy(() => import("@/features/admin/catalog/pages/actors-page"));
const DirectorsPage = lazy(() => import("@/features/admin/catalog/pages/directors-page"));
const AdminMoviesPage = lazy(() => import("@/features/admin/movies/pages/movies-page"));
const AdminBookingsPage = lazy(() => import("@/features/admin/bookings/pages/bookings-page"));
const AdminBookingDetailPage = lazy(() => import("@/features/admin/bookings/pages/booking-detail-page"));

export const adminRoutes: RouteObject[] = [
  {
    path: ROUTES.admin.login,
    element: <GuestRoute redirectTo={ROUTES.admin.dashboard} />,
    children: [
      {
        element: <AdminAuthLayout />,
        children: [{ index: true, element: <LazyPage><AdminLoginPage /></LazyPage> }],
      },
    ],
  },
  {
    path: "/admin",
    element: <AdminProtectedRoute />,
    children: [
      { index: true, element: <AdminIndexRedirect /> },
      {
        element: <AdminLayout />,
        children: [
          { path: "dashboard", element: <LazyPage><AdminDashboardPage /></LazyPage> },
          { path: "movies", element: <LazyPage><AdminMoviesPage /></LazyPage> },
          { path: "showtimes", element: <LazyPage><AdminShowtimesPage /></LazyPage> },
          { path: "bookings", element: <LazyPage><AdminBookingsPage /></LazyPage> },
          { path: "bookings/:id", element: <LazyPage><AdminBookingDetailPage /></LazyPage> },
          { path: "bookings/counter", element: <LazyPage><AdminCounterPage /></LazyPage> },
          { path: "checkin", element: <LazyPage><AdminCheckinPage /></LazyPage> },
          { path: "payments", element: <LazyPage><AdminPaymentsPage /></LazyPage> },
          { path: "reports", element: <LazyPage><AdminReportsPage /></LazyPage> },
          { path: "users", element: <LazyPage><AdminUsersPage /></LazyPage> },
          { path: "employees", element: <LazyPage><AdminEmployeesPage /></LazyPage> },
          { path: "rooms", element: <LazyPage><AdminRoomsPage /></LazyPage> },
          { path: "rooms/:id/seats", element: <LazyPage><AdminRoomSeatsPage /></LazyPage> },
          { path: "combos", element: <LazyPage><AdminCombosPage /></LazyPage> },
          { path: "reviews", element: <LazyPage><AdminReviewsPage /></LazyPage> },
          { path: "settings", element: <LazyPage><AdminSettingsPage /></LazyPage> },
          { path: "catalog", element: <LazyPage><CatalogHubPage /></LazyPage> },
          { path: "catalog/genres", element: <LazyPage><GenresPage /></LazyPage> },
          { path: "catalog/languages", element: <LazyPage><LanguagesPage /></LazyPage> },
          { path: "catalog/age-ratings", element: <LazyPage><AgeRatingsPage /></LazyPage> },
          { path: "catalog/seat-types", element: <LazyPage><SeatTypesPage /></LazyPage> },
          { path: "catalog/screen-types", element: <LazyPage><ScreenTypesPage /></LazyPage> },
          {
            path: "catalog/membership-tiers",
            element: <LazyPage><MembershipTiersPage /></LazyPage>,
          },
          { path: "catalog/positions", element: <LazyPage><PositionsPage /></LazyPage> },
          { path: "catalog/actors", element: <LazyPage><ActorsPage /></LazyPage> },
          { path: "catalog/directors", element: <LazyPage><DirectorsPage /></LazyPage> },
        ],
      },
    ],
  },
];
