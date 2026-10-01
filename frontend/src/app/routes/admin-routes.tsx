import type { RouteObject } from "react-router";

import AdminLayout from "@/layouts/admin/admin-layout";
import { AdminProtectedRoute } from "./protected-route/admin-protected-route";
import { GuestRoute } from "./protected-route/guest-route";

import AdminLoginPage from "@/features/admin/login/pages/login-page";
import AdminDashboardPage from "@/features/admin/dashboard/pages/dashboard-page";
import AdminShowtimesPage from "@/features/admin/showtimes/pages/showtimes-page";
import AdminCounterPage from "@/features/admin/counter/pages/counter-page";
import AdminCheckinPage from "@/features/admin/checkin/pages/checkin-page";
import AdminPaymentsPage from "@/features/admin/payments/pages/payments-page";
import AdminUsersPage from "@/features/admin/users/pages/users-page";
import AdminReportsPage from "@/features/admin/reports/pages/reports.page";
import AdminRoomsPage from "@/features/admin/rooms/pages/rooms-page";
import AdminEmployeesPage from "@/features/admin/employees/pages/employees-page";
import AdminRoomSeatsPage from "@/features/admin/rooms/pages/room-seats-page";
import AdminCombosPage from "@/features/admin/combos/pages/combos-page";
import AdminReviewsPage from "@/features/admin/reviews/pages/reviews-page";
import AdminSettingsPage from "@/features/admin/settings/pages/settings-page";
import CatalogHubPage from "@/features/admin/catalog/pages/catalog-hub-page";
import GenresPage from "@/features/admin/catalog/pages/genres-page";
import LanguagesPage from "@/features/admin/catalog/pages/languages-page";
import AgeRatingsPage from "@/features/admin/catalog/pages/age-ratings-page";
import SeatTypesPage from "@/features/admin/catalog/pages/seat-types-page";
import ScreenTypesPage from "@/features/admin/catalog/pages/screen-types-page";
import MembershipTiersPage from "@/features/admin/catalog/pages/membership-tiers-page";
import PositionsPage from "@/features/admin/catalog/pages/positions-page";
import ActorsPage from "@/features/admin/catalog/pages/actors-page";
import DirectorsPage from "@/features/admin/catalog/pages/directors-page";
import AdminMoviesPage from "@/features/admin/movies/pages/movies-page";
import AdminBookingsPage from "@/features/admin/bookings/pages/bookings-page";
import AdminBookingDetailPage from "@/features/admin/bookings/pages/booking-detail-page";
import AdminAuthLayout from "@/layouts/admin/admin-auth-layout";

export const adminRoutes: RouteObject[] = [
  {
    path: "/admin/login",
    element: <GuestRoute redirectTo="/admin/dashboard" />,
    children: [
      {
        element: <AdminAuthLayout />,
        children: [{ index: true, element: <AdminLoginPage /> }],
      },
    ],
  },
  {
    path: "/admin",
    element: <AdminProtectedRoute />,
    children: [
      {
        element: <AdminLayout />,
        children: [
          { path: "dashboard", element: <AdminDashboardPage /> },
          { path: "movies", element: <AdminMoviesPage /> },
          { path: "showtimes", element: <AdminShowtimesPage /> },
          { path: "bookings", element: <AdminBookingsPage /> },
          { path: "bookings/:id", element: <AdminBookingDetailPage /> },
          { path: "bookings/counter", element: <AdminCounterPage /> },
          { path: "checkin", element: <AdminCheckinPage /> },
          { path: "payments", element: <AdminPaymentsPage /> },
          { path: "reports", element: <AdminReportsPage /> },
          { path: "users", element: <AdminUsersPage /> },
          { path: "employees", element: <AdminEmployeesPage /> },
          { path: "rooms", element: <AdminRoomsPage /> },
          { path: "rooms/:id/seats", element: <AdminRoomSeatsPage /> },
          { path: "combos", element: <AdminCombosPage /> },
          { path: "reviews", element: <AdminReviewsPage /> },
          { path: "settings", element: <AdminSettingsPage /> },
          { path: "catalog", element: <CatalogHubPage /> },
          { path: "catalog/genres", element: <GenresPage /> },
          { path: "catalog/languages", element: <LanguagesPage /> },
          { path: "catalog/age-ratings", element: <AgeRatingsPage /> },
          { path: "catalog/seat-types", element: <SeatTypesPage /> },
          { path: "catalog/screen-types", element: <ScreenTypesPage /> },
          {
            path: "catalog/membership-tiers",
            element: <MembershipTiersPage />,
          },
          { path: "catalog/positions", element: <PositionsPage /> },
          { path: "catalog/actors", element: <ActorsPage /> },
          { path: "catalog/directors", element: <DirectorsPage /> },
        ],
      },
    ],
  },
];
