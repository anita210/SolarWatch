import React, { useContext } from 'react'
import ReactDOM from 'react-dom/client'
import './index.css'
import { RouterProvider, createBrowserRouter } from "react-router-dom";
import Layout from "./Pages/Layout/Layout";
import SolarWatch from "./Pages/SolarWatch/SolarWatch";
import Login from './Pages/Login/Login';
import { useAuth } from './Pages/Layout/Layout';
import { Navigate } from "react-router-dom";
import Register from './Pages/Register/Register';
import Home from './Pages/Home/Home';
import PageLayoutTrial from './Components/PageLayoutTrial/PageLayoutTrial';

const ProtectedRoute = ({ children }) => {
  const { user } = useAuth();
  if (!user) {
    return <Navigate to="/login" />;
  }
  return children;
};

const LoggedInRoute = ({ children }) => {
  const { user } = useAuth();
  if (user) {
    return <Navigate to="/solar-watch" />;
  }
  return children;
};



const router = createBrowserRouter([
  {
    path: '/',
    element: <Layout />,
    children: [
      {
        path: '/home',
        element:
        (<LoggedInRoute>
          <Home />
          </LoggedInRoute>),   
      },
      {
        path: '/login',
        element: (<LoggedInRoute>
          <Login />
          </LoggedInRoute>),
      },
      {
        path: '/register',
        element: (
          <LoggedInRoute>
            <Register />
          </LoggedInRoute>
        ),
      },
      {
        path: '/solar-watch',
        element: (
          <ProtectedRoute>
            <SolarWatch />
          </ProtectedRoute>
        ),
      },
      {
        path: '/trial',
        element: (
            <PageLayoutTrial />
        ),
      }
    ],
  },
]);

ReactDOM.createRoot(document.getElementById("root")).render(
  <React.StrictMode>
    <RouterProvider router={router} />
  </React.StrictMode>
)

