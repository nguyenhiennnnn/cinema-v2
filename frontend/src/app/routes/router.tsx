import { createBrowserRouter, Outlet } from "react-router";
import { clientRoutes } from "./client-routes";
import { adminRoutes } from "./admin-routes";
import NotFoundPage from "@/pages/not-found-page";
import ForbiddenPage from "@/pages/forbidden-page";
import { GlobalModals } from "../modals/global-modals";
import { Toaster } from "@/components/ui/toast";

const RootLayout = () => {
  return (
    <>
      <Outlet />
      <GlobalModals />
      <Toaster />
    </>
  );
};

export const router = createBrowserRouter([
  {
    element: <RootLayout />,
    children: [
      ...clientRoutes,
      ...adminRoutes,
      { path: "/403", element: <ForbiddenPage /> },
      { path: "*", element: <NotFoundPage /> },
    ],
  },
]);