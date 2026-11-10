import { createBrowserRouter, Outlet, useLocation } from "react-router";
import { Helmet } from "react-helmet-async";
import { clientRoutes } from "./client-routes";
import { adminRoutes } from "./admin-routes";
import NotFoundPage from "@/pages/not-found-page";
import ForbiddenPage from "@/pages/forbidden-page";
import ErrorPage from "@/pages/error-page";
import { GlobalModals } from "../modals/global-modals";
import { Toaster } from "@/components/ui/toast";
import { isNoIndexPath } from "@/config/seo";
import { ROUTES } from "@/config/routes";

const RootLayout = () => {
  const { pathname } = useLocation();

  return (
    <>
      {isNoIndexPath(pathname) && (
        <Helmet>
          <meta name="robots" content="noindex, nofollow" />
        </Helmet>
      )}
      <Outlet />
      <GlobalModals />
      <Toaster />
    </>
  );
};

export const router = createBrowserRouter([
  {
    element: <RootLayout />,
    errorElement: <ErrorPage />,
    children: [
      ...clientRoutes,
      ...adminRoutes,
      { path: ROUTES.forbidden, element: <ForbiddenPage /> },
      { path: "*", element: <NotFoundPage /> },
    ],
  },
]);