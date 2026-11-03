import ClientLayout from "@/layouts/client/client-layout";
import BookingPage from "@/features/client/bookings/pages/booking-page";
import BookingResultPage from "@/features/client/bookings/pages/booking-result-page";
import HomePage from "@/features/client/home/pages/home-page";
import MovieDetailPage from "@/features/client/movies/pages/movie-detail-page";
import MoviesPage from "@/features/client/movies/pages/movies-page";
import SchedulePage from "@/features/client/schedule/pages/schedule-page";
import type { RouteObject } from "react-router";
import { CustomerProtectedRoute } from "./protected-route/customer-protected-route";
import { GuestRoute } from "./protected-route/guest-route";
import AccountLayout from "@/layouts/client/account/account-layout";
import AccountProfilePage from "@/features/client/account/pages/profile-page";
import AccountTicketsPage from "@/features/client/account/pages/tickets-page";
import AccountReviewsPage from "@/features/client/account/pages/reviews-page";
import ChangePasswordPage from "@/features/client/account/pages/change-password-page";
import AccountTicketDetailPage from "@/features/client/account/pages/ticket-detail-page";
import AuthLayout from "@/layouts/client/auth-layout";
import LoginPage from "@/features/auth/pages/login-page";
import RegisterPage from "@/features/auth/pages/register-page";
import ConfirmEmailPage from "@/features/auth/pages/confirm-email-page";
import ForgotPasswordPage from "@/features/auth/pages/forgot-password-page";
import ResetPasswordPage from "@/features/auth/pages/reset-password-page";
import MembershipPage from "@/features/client/membership/pages/membership-page";
import PolicyPage from "@/features/client/policies/pages/policy-page";

export const clientRoutes: RouteObject[] = [
  {
    path: "/",
    element: <ClientLayout />,
    children: [
      { index: true, element: <HomePage /> },
      { path: "movies", element: <MoviesPage /> },
      { path: "movies/:id", element: <MovieDetailPage /> },
      { path: "schedule", element: <SchedulePage /> },
      { path: "membership", element: <MembershipPage /> },
      { path: "policies/:slug", element: <PolicyPage /> },
      { path: "booking/:showtimeId", element: <BookingPage /> },
      { path: "booking/result", element: <BookingResultPage /> },
      {
        path: "account",
        element: <CustomerProtectedRoute />,
        children: [
          {
            element: <AccountLayout />,
            children: [
              { path: "profile", element: <AccountProfilePage /> },
              { path: "tickets", element: <AccountTicketsPage /> },
              { path: "reviews", element: <AccountReviewsPage /> },
              { path: "change-password", element: <ChangePasswordPage /> },
            ],
          },
          { path: "tickets/:id", element: <AccountTicketDetailPage /> },
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
          { path: "login", element: <LoginPage /> },
          { path: "register", element: <RegisterPage /> },
          { path: "confirm-email", element: <ConfirmEmailPage /> },
          { path: "forgot-password", element: <ForgotPasswordPage /> },
          { path: "reset-password", element: <ResetPasswordPage /> },
        ],
      },
    ],
  },
];
