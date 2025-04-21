import { BrowserRouter, Route, Routes, useNavigate } from "react-router-dom";
import { AuthProvider } from "./context/AuthContext";
import { setAuthFailureCallback } from "./utils/apiClient";
import { useEffect } from "react";
import LoginPage from "./Pages/LoginPage";
import ConfirmEmailPage from "./Pages/ConfirmEmailPage";
import HomePage from "./Pages/HomePage";
import RegistrationPage from "./Pages/RegistrationPage";
import Layout from "./Components/oth/Layout";
import MessagesPage from "./Pages/MessagesPage";
import FriendsPage from "./Pages/FriendsPage";
import MyPostsPage from "./Pages/MyPostsPage";
import MyProfilePage from "./Pages/MyProfilePage";
import UserProfilePage from "./Pages/UserProfilePage";
import CreatePostPage from "./Pages/CreatePostPage";
import ProtectedRoute from "./Components/oth/ProtectedRoute";
import PublicRoute from "./Components/oth/PublicRoute";
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { UnreadMessagesProvider } from "./context/UnreadMessagesContext";


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
      <UnreadMessagesProvider>
        <AuthProvider>
          <ToastContainer /> {/* Make sure this is here */}
          <AuthCallbackSetter />
          <Routes>
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
            <Route path="confirmEmail" element={
              <PublicRoute>
                <ConfirmEmailPage />
              </PublicRoute>
            } />

            <Route path="/" element={
              <ProtectedRoute>
                <Layout />
              </ProtectedRoute>
            }>
              <Route index element={<HomePage />} />
              <Route path="/messages" element={<MessagesPage />} />
              <Route path="/messages/:conversationId" element={<MessagesPage />} />
              <Route path="/friends" element={<FriendsPage />} />
              <Route path="/my-posts" element={<MyPostsPage />} />
              <Route path="/my-profile" element={<MyProfilePage />} />
              <Route path="/create-post" element={<CreatePostPage />} />
              <Route path="/profile/:userId" element={<UserProfilePage />} />
            </Route>
          </Routes>
        </AuthProvider>
      </UnreadMessagesProvider>
    </BrowserRouter >

  );
}

export default App;
