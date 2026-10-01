import { RouterProvider } from "react-router";

import { AppProviders } from "./app/providers/app-provider";
import { router } from "./app/routes/router";

const App = () => {
  return (
    <AppProviders>
      <RouterProvider router={router} />
    </AppProviders>
  );
};

export default App;

// docker compose exec frontend npm install