import { BrowserRouter, Route, Routes, useNavigate } from "react-router-dom";
import { AuthProvider } from "./context/AuthContext";
import { setAuthFailureCallback } from "./utils/apiClient";
import { useEffect } from "react";
import LoginPage from "./Pages/LoginPage";
import HomePage from "./Pages/HomePage";
import RegistrationPage from "./Pages/RegistrationPage";
import Layout from "./Components/Layout";
import MessagesPage from "./Pages/MessagesPage";
import FriendsPage from "./Pages/FriendsPage";
import MyPostsPage from "./Pages/MyPostsPage";
import MyProfilePage from "./Pages/MyProfilePage";
import CreatePostPage from "./Pages/CreatePostPage";
import ProtectedRoute from "./Components/ProtectedRoute";
import PublicRoute from "./Components/PublicRoute";  // We'll create this component

// Компонент для встановлення колбеку автентифікації
const AuthCallbackSetter = () => {
  const navigate = useNavigate();
  
  useEffect(() => {
    setAuthFailureCallback(() => {
      navigate('/login', { replace: true });
    });
  }, [navigate]);
  
  return null;
};

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <AuthCallbackSetter />
        <Routes>
          {/* Public routes - only for non-authenticated users */}
          <Route path="login" element={
            <PublicRoute>
              <LoginPage />
            </PublicRoute>
          } />
          <Route path="registration" element={
            <PublicRoute>
              <RegistrationPage />
            </PublicRoute>
          } />
          
          {/* Protected routes - only for authenticated users */}
          <Route path="/" element={
            <ProtectedRoute>
              <Layout />
            </ProtectedRoute>
          }>
            <Route index element={<HomePage />} />
            <Route path="/messages" element={<MessagesPage />} />
            <Route path="/friends" element={<FriendsPage />} />
            <Route path="/my-posts" element={<MyPostsPage />} />
            <Route path="/my-profile" element={<MyProfilePage />} />
            <Route path="/create-post" element={<CreatePostPage />} />
          </Route>
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
