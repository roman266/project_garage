import { BrowserRouter, Route, Routes } from "react-router-dom";
import { AuthProvider } from "./context/AuthContext";
import LoginPage from "./Pages/LoginPage";
import HomePage from "./Pages/HomePage";
import RegistrationPage from "./Pages/RegistrationPage";
import Layout from "./Components/Layout";
import MessagesPage from "./Pages/MessagesPage";
import FriendsPage from "./Pages/FriendsPage";
import MyPostsPage from "./Pages/MyPostsPage";
import MyProfilePage from "./Pages/MyProfilePage";
import CreatePostPage from "./Pages/CreatePostPage";
import ProtectedRoute from "./Components/ProtectedRoute";  // New Component for protected routes

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          {/* Public routes */}
          <Route path="login" element={<LoginPage />} />
          <Route path="registration" element={<RegistrationPage />} />
          
          {/* Protected routes - wrap Layout in ProtectedRoute */}
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
