import { Home } from "./components/Home";
import Search from './components/Search';
import SearchHistory from './components/SearchHistory';

const AppRoutes = [
  {
    index: true,
    element: <Search />
  },
  {
    path: '/history',
    element: <SearchHistory />
  }
];

export default AppRoutes;
