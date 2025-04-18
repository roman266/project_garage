import React, { useState, useEffect } from "react";
import { Outlet, Link, useNavigate } from "react-router-dom";
import { API_URL } from "../../constants";
import ProfileCard from "./ProfileCard";
import SearchResults from "./SearchResults";
import Snackbar from '@mui/material/Snackbar';
import Alert from '@mui/material/Alert';
import AlertTitle from '@mui/material/AlertTitle';
import { useUnreadMessages } from "../../context/UnreadMessagesContext"; // Додаємо імпорт контексту

import {
  AppBar,
  Toolbar,
  Typography,
  InputBase,
  Box,
  Drawer,
  List,
  ListItem,
  ListItemText,
  Button,

  ListItemIcon,
  IconButton
} from "@mui/material";
import { useAuth } from "../../context/AuthContext";
import SearchIcon from "@mui/icons-material/Search";
import axios from "axios";

const drawerWidth = 240;

const menuItems = [
  { text: "My profile", imgSrc: "/profile.svg", pageHref: "/my-profile" },
  { text: "Friends", imgSrc: "/friends.svg", pageHref: "/friends" },
  { text: "Messages", imgSrc: "/messages.svg", pageHref: "/messages", hasCounter: true },
  { text: "My posts", imgSrc: "/posts.svg", pageHref: "/my-posts" },
];

const Layout = () => {
  const { user, logout, isLoading } = useAuth();
  const navigate = useNavigate();
  const { unreadCount } = useUnreadMessages(); // Використовуємо контекст для отримання кількості непрочитаних повідомлень

  const [searchQuery, setSearchQuery] = useState("");
  const [searchResults, setSearchResults] = useState([]);
  const [alert, setAlert] = useState(null);
  // Видаляємо локальний стан unreadCount, оскільки тепер використовуємо контекст

  // Handle logout with local navigation
  const handleLogout = async () => {
    try {
      await logout();
      navigate('/login', { replace: true });
    } catch (error) {
      console.error('Logout error:', error);
    }
  };

  const handleSearch = async () => {
    if (!searchQuery.trim()) return;

    try {
      const response = await axios.get(`${API_URL}/api/profile/search-users?query=${searchQuery}`, {
        withCredentials: true
      });

      setSearchResults(response.data.message.$values || []);
      setAlert(null);
    } catch (error) {
      const errorMessage = error.response?.data?.message;

      if (errorMessage === "No users founded") {
        setSearchResults([]);
        setAlert({ type: "info", message: "No users found." });
      } else {
        setSearchResults([]);
        setAlert({ type: "error", message: "Something went wrong. Please try again." });
      }
    }
  };

  if (isLoading) {
    return <div>Loading...</div>;
  }

  return (
    <Box sx={{ display: "flex", height: "100vh", width: "100vw", overflow: "hidden" }}>
      <Drawer
        variant="permanent"
        sx={{
          flexShrink: 0,
          "& .MuiDrawer-paper": {
            width: drawerWidth,
            boxSizing: "border-box",
            backgroundColor: "white",
            color: "black",
          },
        }}
      >
        <Box sx={{ padding: 2 }}>
          <Link to="/" style={{ textDecoration: 'none' }}>
            <Typography
              variant="h6"
              sx={{
                display: "flex",
                alignItems: "center",
                fontSize: "60px",
                fontWeight: "light",
                fontFamily: "roboto",
                color: "#365B87",
                cursor: "pointer",
              }}
            >
              Sigm
              <img src="/sigma_2.svg" alt="Sigma Logo" style={{ height: "60px", marginLeft: "2px" }} />
            </Typography>
          </Link>
        </Box>
        <ProfileCard profile={user} />
        <List>
          {menuItems.map(({ text, imgSrc, pageHref, hasCounter }) => (
            <ListItem key={text} sx={{ cursor: "pointer", paddingY: 1 }}>
              <Link to={pageHref} style={{ textDecoration: 'none', color: 'inherit', width: '100%' }}>
                <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                  <Box sx={{ display: 'flex', alignItems: 'center' }}>
                    <ListItemIcon sx={{ minWidth: 30 }}>
                      <img src={imgSrc} alt={text} style={{ width: 24, height: 24 }} />
                    </ListItemIcon>
                    <ListItemText
                      primary={text}
                      sx={{
                        marginLeft: 1,
                        fontSize: "16px",
                        fontWeight: 500,
                        color: "#2B2B2B",
                      }}
                    />
                  </Box>
                  {hasCounter && unreadCount > 0 && (
                    <Box
                      sx={{
                        backgroundColor: "#e53935",
                        color: "white",
                        borderRadius: "50%",
                        minWidth: "20px",
                        height: "20px",
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "center",
                        fontSize: "12px",
                        fontWeight: "bold",
                      }}
                    >
                      {unreadCount > 99 ? "99+" : unreadCount}
                    </Box>
                  )}
                </Box>
              </Link>
            </ListItem>
          ))}
        </List>
        <Box sx={{ padding: 2 }}>
          <Link to={"create-post"}>
            <Button variant="contained" color="primary" fullWidth sx={{ borderRadius: "40px", backgroundColor: "#365B87" }}>
              Post
            </Button>
          </Link>
          <Button
            variant="contained"
            color="primary"
            fullWidth
            sx={{ borderRadius: "40px", backgroundColor: "#365B87", marginTop: "10px" }}
            onClick={handleLogout}
          >
            Log out
          </Button>
        </Box>
      </Drawer>

      <Box sx={{ flex: 1, display: "flex", flexDirection: "column", height: "100vh", marginLeft: `${drawerWidth}px` }}>
        {/* Header */}
        <AppBar position="fixed" sx={{ backgroundColor: "#2b2b2b", boxShadow: "none", zIndex: 1100 }}>
          <Toolbar>
            <Typography variant="h6" sx={{ flexGrow: 1, color: "white" }}>
              Dashboard
            </Typography>
            <Box
              sx={{
                display: "flex",
                alignItems: "center",
                backgroundColor: "rgba(255, 255, 255, 0.1)",
                padding: "6px 12px",
                borderRadius: "24px",
                position: 'fixed',
                top: '8px',
                right: '16px',
                zIndex: 1101
              }}
            >
              <IconButton onClick={handleSearch}>
                <SearchIcon style={{ color: "white" }} />
              </IconButton>
              <InputBase
                placeholder="Search"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
                sx={{ marginLeft: 1, color: "white" }}
              />
            </Box>
          </Toolbar>
        </AppBar>

        {/* Main Content */}
        <Box
          sx={{
            flex: 1,
            backgroundColor: "#365B87",
            marginTop: "63px",
            overflowY: "auto",
            padding: 2
          }}
        >
          <Outlet />
          <SearchResults results={searchResults} />
        </Box>
      </Box>

      {/* Snackbar Alert */}
      <Snackbar
        open={!!alert}
        autoHideDuration={5000}
        onClose={() => setAlert(null)}
        anchorOrigin={{ vertical: 'top', horizontal: 'center' }}
      >
        {alert && (
          <Alert onClose={() => setAlert(null)} severity={alert.type} sx={{ width: '100%' }}>
            <AlertTitle>{alert.type === "error" ? "Error" : "Info"}</AlertTitle>
            {alert.message}
          </Alert>
        )}
      </Snackbar>
    </Box>
  );
};

export default Layout;