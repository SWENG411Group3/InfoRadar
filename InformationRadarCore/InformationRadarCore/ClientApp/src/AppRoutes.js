import ApiAuthorzationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { Counter } from "./components/Counter";
import { FetchData } from "./components/FetchData";
import { Home } from "./components/Home";
import { LighthousePage } from "./components/LighthousePage";
import { Dashboard } from './components/Dashboard';

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/dashboard',
      requireAuth: true,
      element: <Dashboard />
  },
  {
    path: '/Lighthouse/:id',
    requireAuth: true,
    element: <LighthousePage />
  },
  ...ApiAuthorzationRoutes
];

export default AppRoutes;
