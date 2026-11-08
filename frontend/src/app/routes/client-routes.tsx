import { lazy } from "react";
import type { RouteObject } from "react-router";

import ClientLayout from "@/layouts/client/client-layout";
import AccountLayout from "@/layouts/client/account/account-layout";
import AuthLayout from "@/layouts/client/auth-layout";
import { CustomerProtectedRoute } from "./protected-route/customer-protected-route";
import { GuestRoute } from "./protected-route/guest-route";
import { LazyPage } from "./lazy-page";

const BookingPage = lazy(() => import("@/features/client/bookings/pages/booking-page"));
const BookingResultPage = lazy(() => import("@/features/client/bookings/pages/booking-result-page"));
const HomePage = lazy(() => import("@/features/client/home/pages/home-page"));
const MovieDetailPage = lazy(() => import("@/features/client/movies/pages/movie-detail-page"));
const MoviesPage = lazy(() => import("@/features/client/movies/pages/movies-page"));
const SchedulePage = lazy(() => import("@/features/client/schedule/pages/schedule-page"));
const AccountProfilePage = lazy(() => import("@/features/client/account/pages/profile-page"));
const AccountTicketsPage = lazy(() => import("@/features/client/account/pages/tickets-page"));
const AccountReviewsPage = lazy(() => import("@/features/client/account/pages/reviews-page"));
const ChangePasswordPage = lazy(() => import("@/features/client/account/pages/change-password-page"));
const AccountTicketDetailPage = lazy(() => import("@/features/client/account/pages/ticket-detail-page"));
const LoginPage = lazy(() => import("@/features/auth/pages/login-page"));
const RegisterPage = lazy(() => import("@/features/auth/pages/register-page"));
const ConfirmEmailPage = lazy(() => import("@/features/auth/pages/confirm-email-page"));
const ForgotPasswordPage = lazy(() => import("@/features/auth/pages/forgot-password-page"));
const ResetPasswordPage = lazy(() => import("@/features/auth/pages/reset-password-page"));
const MembershipPage = lazy(() => import("@/features/client/membership/pages/membership-page"));
const PolicyPage = lazy(() => import("@/features/client/policies/pages/policy-page"));

export const clientRoutes: RouteObject[] = [
  {
    path: "/",
    element: <ClientLayout />,
    children: [
      { index: true, element: <LazyPage><HomePage /></LazyPage> },
      { path: "movies", element: <LazyPage><MoviesPage /></LazyPage> },
      { path: "movies/:id", element: <LazyPage><MovieDetailPage /></LazyPage> },
      { path: "schedule", element: <LazyPage><SchedulePage /></LazyPage> },
      { path: "membership", element: <LazyPage><MembershipPage /></LazyPage> },
      { path: "policies/:slug", element: <LazyPage><PolicyPage /></LazyPage> },
      { path: "booking/:showtimeId", element: <LazyPage><BookingPage /></LazyPage> },
      { path: "booking/result", element: <LazyPage><BookingResultPage /></LazyPage> },
      {
        path: "account",
        element: <CustomerProtectedRoute />,
        children: [
          {
            element: <AccountLayout />,
            children: [
              { path: "profile", element: <LazyPage><AccountProfilePage /></LazyPage> },
              { path: "tickets", element: <LazyPage><AccountTicketsPage /></LazyPage> },
              { path: "reviews", element: <LazyPage><AccountReviewsPage /></LazyPage> },
              { path: "change-password", element: <LazyPage><ChangePasswordPage /></LazyPage> },
            ],
          },
          { path: "tickets/:id", element: <LazyPage><AccountTicketDetailPage /></LazyPage> },
        ],
      },
    ],
  },
  {
    path: "/auth",
    element: <GuestRoute redirectTo="/" />,
    children: [
      {
        element: <AuthLayout />,
        children: [
          { path: "login", element: <LazyPage><LoginPage /></LazyPage> },
          { path: "register", element: <LazyPage><RegisterPage /></LazyPage> },
          { path: "confirm-email", element: <LazyPage><ConfirmEmailPage /></LazyPage> },
          { path: "forgot-password", element: <LazyPage><ForgotPasswordPage /></LazyPage> },
          { path: "reset-password", element: <LazyPage><ResetPasswordPage /></LazyPage> },
        ],
      },
    ],
  },
];
