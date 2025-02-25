import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import LoginPage from "./Pages/LoginPage";
import HomePage from "./Pages/HomePage";
import RegistrationPage from "./Pages/RegistrationPage";
import Layout from "./Components/Layout";
import MessagesPage from "./Pages/MessagesPage";
import FriendsPage from "./Pages/FriendsPage";
import MyPostsPage from "./Pages/MyPostsPage";
import MyProfilePage from "./Pages/MyProfilePage";

function App() {
  return (
    <Router>
    <Routes>
      {/* Отдельные маршруты без Layout */}
      <Route path="/login" element={<LoginPage />} />
      <Route path="/registration" element={<RegistrationPage />} />

      {/* Группа маршрутов с Layout */}
      <Route element={<Layout />}>
        <Route path="/" element={<HomePage />} />
        <Route path="/messages" element={<MessagesPage />} />
        <Route path="/friends" element={<FriendsPage />} />
        <Route path="/my-posts" element={<MyPostsPage />} />
        <Route path="/my-profile" element={<MyProfilePage />} />
      </Route>
    </Routes>
  </Router>
  );
}

export default App;
