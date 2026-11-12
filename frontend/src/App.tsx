import { RouterProvider } from "react-router";

import { AppProviders } from "./app/providers/app-provider";
import { router } from "./app/routes/router";
import { useRoutePreload } from "./hooks/use-route-preload";

const App = () => {
  useRoutePreload(router);

  return (
    <AppProviders>
      <RouterProvider router={router} />
    </AppProviders>
  );
};

export default App;
